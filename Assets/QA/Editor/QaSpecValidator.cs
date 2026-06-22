using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QA.Editor
{
    // ─────────────────────────────────────────────
    // QaSpecValidator: qa-spec.json(WHAT) ↔ QaApi(구현) 정합을 검증하는 드리프트 검증기(계약 테스트 역할).
    // API와 명세를 분리했으므로 이 검증기가 둘의 어긋남을 막는 안전장치다.
    //   - 매 스크립트 리로드 자동 실행([DidReloadScripts]) → 드리프트 시 LogError(즉시 포착).
    //   - 수동 실행: 메뉴 Tools/QA/Validate Spec.
    // 검사: ① 구현 표기(implemented:false 아님) 커맨드는 동명 QaApi 메서드 존재 / 미구현 표기(implemented:false) 커맨드는 메서드 부재
    //       ② 메서드 가진(구현) 커맨드는 인자(이름·타입·required) 일치
    //       ③ blocking 라우팅 정합: blocking:true 커맨드는 async(Task 반환) 메서드, 아니면 동기 메서드(qa_call/qa_await 오라우팅·캐스팅 실패 사전 차단)
    //       ④ QaApi public static 메서드는 명세에 등재(노출 불가 메서드 금지). returns 는 문서용(미검증).
    // 구현 여부 기준은 qa-spec.json 의 implemented 플래그(QaSpec.IsImplemented). S3 구현 시 플래그만 떼면 자동 강제된다.
    // ─────────────────────────────────────────────
    public static class QaSpecValidator
    {

        /// <summary>스크립트 리로드 직후 자동 검증한다(드리프트만 보고).</summary>
        [DidReloadScripts]
        private static void OnReload()
        {
            Validate(verbose: false);
        }

        /// <summary>메뉴에서 수동 검증한다(성공/실패 모두 보고).</summary>
        [MenuItem("Tools/QA/Validate Spec")]
        private static void ValidateMenu()
        {
            Validate(verbose: true);
        }

        /// <summary>명세 ↔ QaApi 정합을 검사하고 결과를 콘솔에 보고한다.</summary>
        public static void Validate(bool verbose)
        {
            List<string> errors = new List<string>();
            JObject commands;
            try
            {
                commands = (JObject)QaSpec.Load()["commands"];
            }
            catch (Exception loadError)
            {
                Debug.LogError($"[QA] 드리프트 검증 실패 — 명세 로드 오류: {loadError.Message}");
                return;
            }

            // QaApi 의 커맨드 메서드(상속 멤버 제외).
            MethodInfo[] methods = typeof(QaApi)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            int implementedChecked = 0;
            foreach (JProperty commandProp in commands.Properties())
            {
                string name = commandProp.Name;
                JObject node = (JObject)commandProp.Value;
                bool declaredImplemented = QaSpec.IsImplemented(node);
                MethodInfo method = methods.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));

                if (method == null)
                {
                    // 미구현 표기(implemented:false)는 메서드 부재가 정상 — 구현 표기 커맨드만 존재를 강제한다.
                    if (declaredImplemented)
                    {
                        errors.Add($"구현 표기 커맨드 '{name}' 에 대응하는 QaApi 메서드가 없습니다(메서드를 추가하거나 implemented:false 로 표기하세요).");
                    }
                }
                else if (!declaredImplemented)
                {
                    // 메서드가 있는데 미구현 표기면 플래그↔실제가 어긋남.
                    errors.Add($"미구현 표기(implemented:false) 커맨드 '{name}' 에 QaApi 메서드가 존재합니다(구현됐다면 implemented:false 플래그를 제거하세요).");
                }
                else
                {
                    implementedChecked++;
                    ValidateSignature(name, node, method, errors);
                }
            }

            // 명세에 없는 public static 메서드(=노출 불가) 금지.
            foreach (MethodInfo method in methods)
            {
                bool inSpec = commands.Properties()
                    .Any(p => string.Equals(p.Name, method.Name, StringComparison.OrdinalIgnoreCase));
                if (!inSpec)
                {
                    errors.Add($"QaApi 메서드 '{method.Name}' 가 명세에 없습니다(qa_call 노출 불가 — 명세 등재 또는 비공개화).");
                }
            }

            if (errors.Count > 0)
            {
                Debug.LogError($"[QA] spec ↔ QaApi 드리프트 {errors.Count}건:\n  - " + string.Join("\n  - ", errors));
            }
            else if (verbose)
            {
                Debug.Log($"[QA] spec ↔ QaApi 정합 OK (구현 슬라이스 메서드 {implementedChecked}개 검증).");
            }
        }

        /// <summary>한 커맨드의 명세 parameters 와 메서드 시그니처(이름·타입·required) 정합을 검사한다.</summary>
        private static void ValidateSignature(string name, JObject node, MethodInfo method, List<string> errors)
        {
            JObject specParams = node["parameters"] as JObject;
            JObject specProperties = specParams?["properties"] as JObject ?? new JObject();
            HashSet<string> requiredNames = new HashSet<string>(
                (specParams?["required"] as JArray)?.Select(t => t.ToString()) ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            ParameterInfo[] methodParams = method.GetParameters();
            HashSet<string> methodParamNames = new HashSet<string>(methodParams.Select(p => p.Name), StringComparer.OrdinalIgnoreCase);

            // 명세 인자 ↔ 메서드 인자 집합 일치.
            foreach (JProperty specProp in specProperties.Properties())
            {
                if (!methodParamNames.Contains(specProp.Name))
                {
                    errors.Add($"'{name}': 명세 인자 '{specProp.Name}' 가 메서드 시그니처에 없습니다.");
                }
            }

            foreach (ParameterInfo parameter in methodParams)
            {
                JObject specProp = specProperties[FindKeyCI(specProperties, parameter.Name)] as JObject;
                if (specProp == null)
                {
                    errors.Add($"'{name}': 메서드 인자 '{parameter.Name}' 가 명세 parameters 에 없습니다.");
                    continue;
                }

                // 타입 정합(JSON 스키마 기준).
                string expected = ExpectedJsonType(specProp);
                string actual = JsonTypeName(parameter.ParameterType);
                if (expected != actual)
                {
                    errors.Add($"'{name}': 인자 '{parameter.Name}' 타입 불일치(명세 {expected} ≠ 메서드 {actual}).");
                }

                // required 정합: 명세 required 면 기본값 없어야, 비-required 면 기본값 있어야.
                bool isRequired = requiredNames.Contains(parameter.Name);
                if (isRequired && parameter.HasDefaultValue)
                {
                    errors.Add($"'{name}': 인자 '{parameter.Name}' 는 명세상 required 인데 메서드에 기본값이 있습니다.");
                }
                else if (!isRequired && !parameter.HasDefaultValue)
                {
                    errors.Add($"'{name}': 인자 '{parameter.Name}' 는 명세상 선택인데 메서드에 기본값이 없습니다.");
                }
            }

            // 블로킹 라우팅 정합: blocking:true → async(Task 반환), 아니면 동기. qa_call(동기)/qa_await(블로킹) 오라우팅과
            // qa_await 의 Task<object> 캐스팅 실패를 사전에 막는다(벤더도 typeof(Task).IsAssignableFrom 으로 async 등록 판정).
            bool declaredBlocking = QaSpec.IsBlocking(node);
            bool methodIsAsync = typeof(Task).IsAssignableFrom(method.ReturnType);
            if (declaredBlocking && !methodIsAsync)
            {
                errors.Add($"'{name}': blocking:true 인데 메서드 반환이 동기입니다 — 블로킹 커맨드는 async Task<object> 시그니처여야 합니다.");
            }
            else if (!declaredBlocking && methodIsAsync)
            {
                errors.Add($"'{name}': 메서드가 async(Task 반환)인데 spec 에 blocking:true 표기가 없습니다 — blocking:true 를 추가하거나 동기로 바꾸세요.");
            }
        }

        /// <summary>명세 파라미터 노드의 기대 JSON 타입(type 또는 $ref→object).</summary>
        private static string ExpectedJsonType(JObject specProp)
        {
            string type = (string)specProp["type"];
            if (!string.IsNullOrEmpty(type))
            {
                return type;
            }
            // $ref / allOf 등 구조 참조는 object 로 본다.
            return "object";
        }

        /// <summary>C# 타입을 JSON 스키마 표기로 바꾼다.</summary>
        private static string JsonTypeName(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long)) return "integer";
            if (type == typeof(float) || type == typeof(double)) return "number";
            if (type == typeof(bool)) return "boolean";
            return "object";
        }

        /// <summary>JObject 에서 키를 대소문자 무시로 찾아 실제 키명을 반환한다(없으면 null).</summary>
        private static string FindKeyCI(JObject obj, string key)
        {
            return obj.Properties()
                .FirstOrDefault(p => string.Equals(p.Name, key, StringComparison.OrdinalIgnoreCase))?.Name;
        }
    }
}
