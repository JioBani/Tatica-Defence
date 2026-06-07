using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MCPForUnity.Editor.Helpers;
using MCPForUnity.Editor.Tools;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace QA.Editor
{
    // ─────────────────────────────────────────────
    // QaDispatch: qa-spec.json 을 단일 진실로, QA 파사드(QaApi)를 네이티브 MCP 도구로 노출하는 게이트웨이.
    // 노출 도구는 qa_index/qa_call/qa_await 3개로 고정 → 개별 커맨드는 args 안에서 자유(도구 시그니처 stickiness 무관).
    //   - qa_index: 명세 제공(무인자=압축 목록 / {command}=풀 스키마). 발견 진입점.
    //   - qa_call : 동기 커맨드 디스패치. command 조회(=노출 게이트) → args 검증 → 동명 QaApi 메서드 리플렉션 호출.
    //   - qa_await: 블로킹(대기형, spec blocking:true) 커맨드 전용 async 디스패치(응답 hold-open). Step·RunUntil 등.
    //     벤더 CommandRegistry 가 typeof(Task).IsAssignableFrom 으로 async 등록을 판정하므로 HandleCommand 는 async Task<object>.
    // command 해석(노출·구현·블로킹 게이트 + 인자 바인딩)은 QaRouter.Resolve 로 공유 — qa_call/qa_await 는 호출·봉투만 분기.
    // 공통 봉투: 성공 { ok:true, data } / 실패 { ok:false, error }.
    // 새 커맨드 추가는 QaApi 메서드 + qa-spec.json 등재만 — 이 파일은 손대지 않는다(드리프트 검증기가 정합 강제).
    // ─────────────────────────────────────────────

    /// <summary>qa-spec.json 로드·조회 및 게이트웨이 공통 헬퍼.</summary>
    internal static class QaSpec
    {
        /// <summary>명세 파일 절대 경로(Assets/QA/Editor/qa-spec.json).</summary>
        public static string Path => System.IO.Path.Combine(Application.dataPath, "QA", "Editor", "qa-spec.json");

        /// <summary>명세 파일을 파싱해 반환한다. 호출마다 읽어 항상 최신.</summary>
        public static JObject Load()
        {
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException($"qa-spec.json 을 찾을 수 없습니다: {Path}");
            }
            return JObject.Parse(File.ReadAllText(Path));
        }

        /// <summary>commands 노드에서 이름을 대소문자 무시로 조회한다(없으면 null).</summary>
        public static JProperty FindCommand(JObject commands, string name)
        {
            return commands?.Properties()
                .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>JObject 에서 키를 대소문자 무시로 읽는다.</summary>
        public static JToken Get(JObject obj, string key)
        {
            return obj?.GetValue(key, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>커맨드가 구현됨으로 표기됐는지 판정한다(implemented:false 가 아니면 구현됨). qa_call 구현 게이트·드리프트 검증 공용 기준.</summary>
        public static bool IsImplemented(JObject commandNode)
        {
            JToken flag = Get(commandNode, "implemented");
            return flag == null || flag.Type == JTokenType.Null || flag.Value<bool>();
        }

        /// <summary>커맨드가 블로킹(대기형, blocking:true)인지 판정한다. 동기 qa_call vs async qa_await 라우팅·드리프트 검증 공용 기준.</summary>
        public static bool IsBlocking(JObject commandNode)
        {
            JToken flag = Get(commandNode, "blocking");
            return flag != null && flag.Type != JTokenType.Null && flag.Value<bool>();
        }

        // 봉투는 게이트웨이(벤더 MCP)의 단일 봉투만 쓴다 — 성공 SuccessResponse{success,data} / 실패 ErrorResponse{success:false,error}.
        // 자체 {ok} 레이어를 두지 않아 에이전트가 이중 봉투를 보지 않는다.
        public static object Ok(object data)
        {
            return new SuccessResponse("ok", data);
        }

        public static object Error(string message)
        {
            return new ErrorResponse(message);
        }

        /// <summary>스펙 파라미터 노드의 표시용 타입명(type 또는 $ref 의 끝 이름).</summary>
        public static string ParamTypeName(JObject paramNode)
        {
            if (paramNode == null)
            {
                return "any";
            }
            string type = (string)paramNode["type"];
            if (!string.IsNullOrEmpty(type))
            {
                return type;
            }
            string reference = (string)paramNode["$ref"];
            if (!string.IsNullOrEmpty(reference))
            {
                return reference.Split('/').Last();
            }
            return "object";
        }
    }

    /// <summary>qa_call/qa_await 공통 — command+args 를 QaApi 메서드 호출 직전까지 해석한다(노출·구현·블로킹 게이트 + 인자 바인딩).</summary>
    internal static class QaRouter
    {
        /// <summary>해석 결과. 실패 시 Ok=false 이고 Error 에 거부 봉투가 담긴다.</summary>
        internal readonly struct ResolveResult
        {
            public readonly bool Ok;
            public readonly object Error;
            public readonly string Canonical;
            public readonly MethodInfo Method;
            public readonly object[] CallArgs;

            private ResolveResult(bool ok, object error, string canonical, MethodInfo method, object[] callArgs)
            {
                Ok = ok;
                Error = error;
                Canonical = canonical;
                Method = method;
                CallArgs = callArgs;
            }

            public static ResolveResult Fail(object error)
            {
                return new ResolveResult(false, error, null, null, null);
            }

            public static ResolveResult Success(string canonical, MethodInfo method, object[] callArgs)
            {
                return new ResolveResult(true, null, canonical, method, callArgs);
            }
        }

        /// <summary>command 를 검증·라우팅해 호출 준비를 마친다. expectBlocking 으로 동기/블로킹 경로를 강제한다.</summary>
        internal static ResolveResult Resolve(JObject @params, bool expectBlocking)
        {
            string command = QaSpec.Get(@params, "command")?.ToString();
            if (string.IsNullOrEmpty(command))
            {
                Debug.LogWarning("[QA] 호출에 command 가 비어있음.");
                return ResolveResult.Fail(QaSpec.Error("'command' 인자는 필수입니다. qa_index 로 커맨드명을 확인하세요."));
            }

            JObject spec = QaSpec.Load();
            JObject commands = (JObject)spec["commands"];

            // 노출 게이트: 스펙에 등재된 커맨드만 호출 가능.
            JProperty commandProp = QaSpec.FindCommand(commands, command);
            if (commandProp == null)
            {
                Debug.LogWarning($"[QA] 미존재 QA 커맨드 '{command}'.");
                return ResolveResult.Fail(QaSpec.Error($"미존재 커맨드 '{command}'. qa_index 로 사용 가능 커맨드를 확인하세요."));
            }

            string canonical = commandProp.Name;
            JObject commandNode = (JObject)commandProp.Value;

            // 구현 게이트: implemented:false(미구현) 커맨드는 리플렉션 시도 없이 즉시 깔끔하게 거부한다.
            if (!QaSpec.IsImplemented(commandNode))
            {
                Debug.LogWarning($"[QA] 미구현 커맨드 '{canonical}' 호출 시도.");
                return ResolveResult.Fail(QaSpec.Error($"미구현 커맨드 '{canonical}' — 명세에는 있으나 아직 구현되지 않았습니다(implemented:false)."));
            }

            // 블로킹 라우팅 게이트: 동기 qa_call(블로킹 불가)과 async qa_await(블로킹 전용) 경로를 분리한다.
            bool isBlocking = QaSpec.IsBlocking(commandNode);
            if (isBlocking && !expectBlocking)
            {
                Debug.LogWarning($"[QA] 블로킹 커맨드 '{canonical}' 를 qa_call 로 호출 시도.");
                return ResolveResult.Fail(QaSpec.Error($"커맨드 '{canonical}' 는 블로킹(대기형) 커맨드입니다. qa_await 로 호출하세요."));
            }
            if (!isBlocking && expectBlocking)
            {
                Debug.LogWarning($"[QA] 비블로킹 커맨드 '{canonical}' 를 qa_await 로 호출 시도.");
                return ResolveResult.Fail(QaSpec.Error($"커맨드 '{canonical}' 는 동기 커맨드입니다. qa_call 로 호출하세요(qa_await 는 블로킹 전용)."));
            }

            JObject args = QaSpec.Get(@params, "args") as JObject ?? new JObject();

            // 명세 required 누락 검사(케이스 인센서티브).
            JObject specParams = commandNode["parameters"] as JObject;
            JObject specProperties = specParams?["properties"] as JObject;
            JArray required = specParams?["required"] as JArray;
            if (required != null)
            {
                foreach (JToken requiredToken in required)
                {
                    string requiredName = requiredToken.ToString();
                    if (QaSpec.Get(args, requiredName) == null)
                    {
                        string typeName = QaSpec.ParamTypeName(specProperties?[requiredName] as JObject);
                        Debug.LogWarning($"[QA] '{canonical}' 필수 인자 '{requiredName}'({typeName}) 누락.");
                        return ResolveResult.Fail(QaSpec.Error($"커맨드 '{canonical}'의 필수 인자 '{requiredName}'({typeName})가 누락되었습니다."));
                    }
                }
            }

            // 동명 QaApi 메서드 라우팅(드리프트 검증기가 존재·정합을 보장).
            MethodInfo method = typeof(QaApi)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => string.Equals(m.Name, canonical, StringComparison.OrdinalIgnoreCase));
            if (method == null)
            {
                Debug.LogWarning($"[QA] 커맨드 '{canonical}' 에 대응하는 QaApi 메서드가 없음(드리프트).");
                return ResolveResult.Fail(QaSpec.Error($"커맨드 '{canonical}' 구현(QaApi 메서드)이 없습니다. 드리프트 검증기를 확인하세요."));
            }

            // args → 메서드 파라미터 바인딩(이름 기준·대소문자 무시 + 타입 변환).
            ParameterInfo[] parameters = method.GetParameters();
            object[] callArgs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                JToken valueToken = QaSpec.Get(args, parameter.Name);
                if (valueToken == null || valueToken.Type == JTokenType.Null)
                {
                    if (parameter.HasDefaultValue)
                    {
                        callArgs[i] = parameter.DefaultValue;
                    }
                    else
                    {
                        Debug.LogWarning($"[QA] '{canonical}' 인자 '{parameter.Name}'({JsonTypeName(parameter.ParameterType)}) 누락.");
                        return ResolveResult.Fail(QaSpec.Error($"커맨드 '{canonical}'의 인자 '{parameter.Name}'({JsonTypeName(parameter.ParameterType)})가 누락되었습니다."));
                    }
                }
                else
                {
                    try
                    {
                        callArgs[i] = valueToken.ToObject(parameter.ParameterType);
                    }
                    catch (Exception conversionError)
                    {
                        string expected = JsonTypeName(parameter.ParameterType);
                        Debug.LogWarning($"[QA] '{canonical}' 인자 '{parameter.Name}' 타입 변환 실패: {conversionError.Message}");
                        return ResolveResult.Fail(QaSpec.Error($"커맨드 '{canonical}'의 인자 '{parameter.Name}'는 {expected} 를 기대합니다(받은 값: {valueToken})."));
                    }
                }
            }

            return ResolveResult.Success(canonical, method, callArgs);
        }

        /// <summary>C# 타입을 JSON 스키마 표기로 바꾼다(에러 메시지용).</summary>
        internal static string JsonTypeName(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long)) return "integer";
            if (type == typeof(float) || type == typeof(double)) return "number";
            if (type == typeof(bool)) return "boolean";
            return "object";
        }
    }

    /// <summary>사용 가능한 QA 커맨드 인덱스를 명세에서 제공한다(발견 진입점).</summary>
    [McpForUnityTool(
        "qa_index",
        Description = "사용 가능한 QA 커맨드 인덱스를 명세(qa-spec.json)에서 제공한다. 무인자=압축 목록(이름·인자·설명), {command}=그 커맨드 풀 스키마(parameters·returns). qa_call/qa_await 전에 이걸로 발견한다.",
        Group = "core")]
    public static class QaIndex
    {
        /// <summary>qa_index 입력 스키마.</summary>
        public class Parameters
        {
            /// <summary>상세를 볼 커맨드명. 생략하면 전체 압축 목록.</summary>
            [ToolParameter("상세 스키마를 볼 커맨드명(생략 시 전체 압축 목록).", Required = false)]
            public string command { get; set; }
        }

        /// <summary>에이전트가 qa_index 를 호출할 때 진입한다.</summary>
        public static object HandleCommand(JObject @params)
        {
            JObject spec = QaSpec.Load();
            JObject commands = (JObject)spec["commands"];
            string command = QaSpec.Get(@params, "command")?.ToString();

            if (string.IsNullOrEmpty(command))
            {
                // 무인자: 커맨드별 한 줄 요약(이름·인자·설명)을 압축 목록으로.
                var lines = commands.Properties()
                    .Select(p => FormatLine(p.Name, (JObject)p.Value))
                    .ToList();
                return QaSpec.Ok(new Dictionary<string, object>
                {
                    ["commands"] = lines,
                    ["hint"] = "상세 스키마는 qa_index{command} / 동기 호출은 qa_call{command, args} / 블로킹([blocking]) 호출은 qa_await{command, args}.",
                });
            }

            JProperty target = QaSpec.FindCommand(commands, command);
            if (target == null)
            {
                return QaSpec.Error($"미존재 커맨드 '{command}'. 무인자 qa_index 로 목록을 확인하세요.");
            }
            // {command}: 그 커맨드의 풀 스키마(설명·parameters·returns) 그대로.
            return QaSpec.Ok(target.Value);
        }

        /// <summary>커맨드 1개를 'Name(p:type, ...) [slice][blocking] — 설명' 한 줄로 포맷한다.</summary>
        private static string FormatLine(string name, JObject node)
        {
            var parts = new List<string>();
            JObject properties = node["parameters"]?["properties"] as JObject;
            if (properties != null)
            {
                foreach (JProperty prop in properties.Properties())
                {
                    parts.Add($"{prop.Name}:{QaSpec.ParamTypeName((JObject)prop.Value)}");
                }
            }
            string slice = (string)node["slice"];
            string description = (string)node["description"];
            // 블로킹 커맨드는 qa_await 경로임을 한 줄 목록에서도 드러낸다(에이전트가 디스패치 도구를 바로 고르게).
            string blockingTag = QaSpec.IsBlocking(node) ? "[blocking]" : "";
            return $"{name}({string.Join(", ", parts)}) [{slice}]{blockingTag} — {description}";
        }
    }

    /// <summary>동기 command 로 QaApi 메서드를 디스패치 호출한다. qa_call{command, args}.</summary>
    [McpForUnityTool(
        "qa_call",
        Description = "동기 QA 커맨드를 디스패치 호출한다. command(string)=커맨드명(qa_index 참조), args(object)=인자 맵. 예: {command:\"PlaceUnit\", args:{unitInstanceId:123, position:{lane:0, column:1}}}. 블로킹([blocking]) 커맨드는 qa_await 로. 봉투: {ok:true,data}/{ok:false,error}.",
        Group = "core")]
    public static class QaCall
    {
        /// <summary>qa_call 입력 스키마(2개 고정).</summary>
        public class Parameters
        {
            /// <summary>호출할 커맨드명(qa_index 의 이름).</summary>
            [ToolParameter("호출할 동기 QA 커맨드명(qa_index 의 이름).", Required = true)]
            public string command { get; set; }

            /// <summary>커맨드 인자 맵(이름→값). 무인자 커맨드는 생략.</summary>
            [ToolParameter("커맨드 인자 맵(이름→값). 무인자 커맨드는 생략.", Required = false)]
            public object args { get; set; }
        }

        /// <summary>에이전트가 qa_call 을 호출할 때 진입한다.</summary>
        public static object HandleCommand(JObject @params)
        {
            QaRouter.ResolveResult resolved = QaRouter.Resolve(@params, expectBlocking: false);
            if (!resolved.Ok)
            {
                return resolved.Error;
            }

            // 호출 + 봉투 래핑. 파사드 실행 예외는 InnerException 을 풀어 보고한다.
            try
            {
                object result = resolved.Method.Invoke(null, resolved.CallArgs);
                return QaSpec.Ok(result);
            }
            catch (TargetInvocationException invocationError)
            {
                Exception inner = invocationError.InnerException ?? invocationError;
                Debug.LogWarning($"[QA] qa_call '{resolved.Canonical}' 실행 예외: {inner.Message}");
                return QaSpec.Error($"커맨드 '{resolved.Canonical}' 실행 중 예외: {inner.Message}");
            }
        }
    }

    /// <summary>블로킹(대기형, blocking:true) command 를 async 로 디스패치하고 완료까지 응답을 hold-open 한다. qa_await{command, args}.</summary>
    [McpForUnityTool(
        "qa_await",
        Description = "블로킹(대기형) QA 커맨드를 디스패치한다. 완료까지 응답을 열어둔다(매프레임 양보, 단일 hold ≤28s). command(string)=qa_index 의 [blocking] 커맨드명, args(object)=인자 맵. 예: {command:\"Step\", args:{frames:3}}. 동기 커맨드는 qa_call 로. 봉투: {ok:true,data}/{ok:false,error}.",
        Group = "core")]
    public static class QaAwait
    {
        /// <summary>qa_await 입력 스키마(2개 고정, qa_call 과 동일 형태).</summary>
        public class Parameters
        {
            /// <summary>호출할 블로킹 커맨드명(qa_index 의 [blocking] 이름).</summary>
            [ToolParameter("호출할 블로킹 QA 커맨드명(qa_index 의 [blocking] 이름).", Required = true)]
            public string command { get; set; }

            /// <summary>커맨드 인자 맵(이름→값). 무인자 커맨드는 생략.</summary>
            [ToolParameter("커맨드 인자 맵(이름→값). 무인자 커맨드는 생략.", Required = false)]
            public object args { get; set; }
        }

        /// <summary>에이전트가 qa_await 를 호출할 때 진입한다(벤더가 Task 반환으로 async 등록).</summary>
        public static async Task<object> HandleCommand(JObject @params)
        {
            QaRouter.ResolveResult resolved = QaRouter.Resolve(@params, expectBlocking: true);
            if (!resolved.Ok)
            {
                return resolved.Error;
            }

            // 블로킹 커맨드는 async Task<object> 시그니처 — Task 로 받아 완료까지 await(응답 hold-open).
            // 파사드 실행 예외(동기/비동기 모두)는 await 시점에 표출되므로 여기서 잡아 동기 경로와 같은 Error 봉투로 보고한다.
            try
            {
                object raw = resolved.Method.Invoke(null, resolved.CallArgs);
                if (raw is not Task<object> task)
                {
                    Debug.LogWarning($"[QA] 블로킹 커맨드 '{resolved.Canonical}' 가 Task<object> 를 반환하지 않음(시그니처 드리프트).");
                    return QaSpec.Error($"커맨드 '{resolved.Canonical}' 는 블로킹인데 async Task<object> 시그니처가 아닙니다(드리프트 검증기를 확인하세요).");
                }

                object result = await task;
                return QaSpec.Ok(result);
            }
            catch (Exception executionError)
            {
                Exception inner = (executionError as TargetInvocationException)?.InnerException ?? executionError;
                Debug.LogWarning($"[QA] qa_await '{resolved.Canonical}' 실행 예외: {inner.Message}");
                return QaSpec.Error($"커맨드 '{resolved.Canonical}' 실행 중 예외: {inner.Message}");
            }
        }
    }
}
