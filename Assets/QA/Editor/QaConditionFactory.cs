using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Unit.Defenders;
using Scenes.Battle.Feature.Unit.Summoners;
using Scenes.Battle.Feature.Units;
using UnityEngine;

namespace QA.Editor
{
    // ─────────────────────────────────────────────
    // QaConditionFactory: RunUntil 조건 명세(JObject {type, ...params})를 평가 probe(Func<object>)로 변환한다(T8 RunUntil).
    //   - 닫힌 판별 유니온: 등록된 type 만 지원한다. 미지 type/누락 파라미터는 LogError + 예외로 거부한다(조용한 실패 금지).
    //   - 각 case 는 Build 호출 시점(=RunUntil 시작)에 기준선을 스냅샷하는 클로저를 반환한다(원칙 3 — 기준선+엣지).
    //     probe()는 매 프레임 호출돼 충족 시 matchedCondition(Dictionary)을, 미충족 시 null 을 반환한다.
    //   - 엔진(QaWaitEngine)은 이 probe 만 받고 조건 종류를 모른다(개방-폐쇄).
    // 새 조건 추가: 아래 switch 에 case 추가 + qa-spec.json schemas/input/Condition oneOf 에 등재(드리프트는 RunUntil 단일 커맨드라 자동).
    // ─────────────────────────────────────────────
    internal static class QaConditionFactory
    {
        /// <summary>ActionState 의 Downed 상태명(문자열 비교 — 게임 enum 타입 비의존).</summary>
        private const string DownedStateName = "Downed";

        /// <summary>조건 명세를 매 프레임 평가할 probe(충족 시 matchedCondition, 아니면 null)로 빌드한다. 미지원 type 은 거부한다.</summary>
        internal static Func<object> Build(JObject condition)
        {
            string type = QaSpec.Get(condition, "type")?.ToString();
            if (string.IsNullOrEmpty(type))
            {
                Debug.LogError("[QA] RunUntil 조건에 type 이 없습니다.");
                throw new ArgumentException("RunUntil 조건에 type 이 없습니다.");
            }

            switch (type)
            {
                case "phaseEntered":
                    return BuildPhaseTransition(condition, entering: true);
                case "phaseExited":
                    return BuildPhaseTransition(condition, entering: false);
                case "aggressorDied":
                    return BuildAggressorDied();
                case "summonerDowned":
                    return BuildDownedNew(DownedScope.Summoner);
                case "defenderDowned":
                    return BuildDownedNew(DownedScope.Defender);
                default:
                    Debug.LogError($"[QA] 미지원 RunUntil 조건 type '{type}' — 조건 유니온에 등록되지 않았습니다.");
                    throw new NotSupportedException($"미지원 RunUntil 조건 type '{type}'.");
            }
        }

        // ── phaseEntered / phaseExited ──

        /// <summary>지정 페이즈로의 진입(entering)·이탈(!entering) 전이를 기준선 이후 엣지로 검출하는 probe 를 만든다.</summary>
        private static Func<object> BuildPhaseTransition(JObject condition, bool entering)
        {
            RoundManager round = RoundManager.Instance;
            if (round == null)
            {
                Debug.LogError("[QA] RunUntil phase 조건 — RoundManager 가 없습니다.");
                throw new InvalidOperationException("RoundManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            string phase = QaSpec.Get(condition, "phase")?.ToString();
            if (string.IsNullOrEmpty(phase))
            {
                Debug.LogError("[QA] phaseEntered/phaseExited 조건에 phase 인자가 없습니다.");
                throw new ArgumentException("phaseEntered/phaseExited 조건은 phase 인자가 필요합니다.");
            }

            // phase 유효성: 실제 게임 페이즈 enum 에 정의된 값인지 런타임 타입으로 검사(enum 타입명 의존 없음).
            Type phaseEnumType = round.CurrentState.GetType();
            if (!Enum.IsDefined(phaseEnumType, phase))
            {
                Debug.LogError($"[QA] 미지 phase '{phase}' — 유효한 PhaseType 이 아닙니다.");
                throw new ArgumentException($"미지 phase '{phase}' — 유효한 PhaseType 이 아닙니다.");
            }

            string transitionType = entering ? "phaseEntered" : "phaseExited";
            // 기준선: 직전 폴의 phase. 첫 폴은 호출 시점 현재 phase 로 시드 → 전이 엣지만 검출(이미 그 phase 여도 이탈 후 재진입까지 대기).
            string previous = round.CurrentState.ToString();
            return () =>
            {
                string current = round.CurrentState.ToString();
                bool matched = entering
                    ? previous != phase && current == phase
                    : previous == phase && current != phase;
                previous = current;
                if (matched)
                {
                    return new Dictionary<string, object> { ["type"] = transitionType, ["phase"] = phase };
                }
                return null;
            };
        }

        // ── aggressorDied ──

        /// <summary>침략자 잔존 수의 프레임 단위 감소(단일 죽음)를 검출하는 probe 를 만든다. 스폰 증가와 무관하게 감소 엣지만.</summary>
        private static Func<object> BuildAggressorDied()
        {
            RoundAggressorManager manager = UnityEngine.Object.FindAnyObjectByType<RoundAggressorManager>();
            if (manager == null)
            {
                Debug.LogError("[QA] aggressorDied 조건 — RoundAggressorManager 가 없습니다.");
                throw new InvalidOperationException("RoundAggressorManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
            }

            // 직전 폴 값 추적 — 기준선=호출 시점 현재 잔존 수. 감소가 보이면 그 프레임에 죽음이 일어난 것.
            int previous = manager.RemainingAggressorCount;
            return () =>
            {
                int now = manager.RemainingAggressorCount;
                bool died = now < previous;
                previous = now;
                if (died)
                {
                    return new Dictionary<string, object> { ["type"] = "aggressorDied", ["remaining"] = now };
                }
                return null;
            };
        }

        // ── summonerDowned / defenderDowned ──

        /// <summary>Downed 검출 대상 범위(소환술사/전장 소환수).</summary>
        private enum DownedScope { Summoner, Defender }

        /// <summary>기준선에 없던 유닛이 새로 Downed 로 진입하는 순간을 검출하는 probe 를 만든다(재무장 시 기존 Downed 무시 — 원칙 3).</summary>
        private static Func<object> BuildDownedNew(DownedScope scope)
        {
            Func<IEnumerable<Unit>> source = ResolveDownedSource(scope);
            string type = scope == DownedScope.Summoner ? "summonerDowned" : "defenderDowned";

            // 기준선: 호출 시점에 이미 Downed 인 인스턴스는 무시 대상이다(재무장 안전).
            HashSet<int> baseline = new HashSet<int>(DownedIds(source));
            return () =>
            {
                foreach (Unit unit in source())
                {
                    if (unit != null && IsDowned(unit))
                    {
                        int id = unit.gameObject.GetInstanceID();
                        if (!baseline.Contains(id))
                        {
                            return new Dictionary<string, object>
                            {
                                ["type"] = type,
                                ["instanceId"] = id,
                                ["name"] = UnitName(unit),
                            };
                        }
                    }
                }
                return null;
            };
        }

        /// <summary>scope 에 맞는 유닛 소스(소환술사 / 전장 배치 소환수)를 매니저에서 캡처한다. 매니저 부재는 거부.</summary>
        private static Func<IEnumerable<Unit>> ResolveDownedSource(DownedScope scope)
        {
            if (scope == DownedScope.Summoner)
            {
                SummonerManager manager = SummonerManager.Instance;
                if (manager == null)
                {
                    Debug.LogError("[QA] summonerDowned 조건 — SummonerManager 가 없습니다.");
                    throw new InvalidOperationException("SummonerManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
                }
                return () => manager.SpawnedSummoners;
            }
            else
            {
                DefenderManager manager = UnityEngine.Object.FindAnyObjectByType<DefenderManager>();
                if (manager == null)
                {
                    Debug.LogError("[QA] defenderDowned 조건 — DefenderManager 가 없습니다.");
                    throw new InvalidOperationException("DefenderManager 가 없음 — 플레이모드/Battle 씬 로드를 확인하세요.");
                }
                // 죽음은 전장에서만 일어난다(대기석은 Waiting 무적). 전장 배치 소환수만 본다.
                return () => manager.GetBattleAreaDefenders();
            }
        }

        /// <summary>현재 Downed 상태인 인스턴스 id 들을 열거한다(기준선 스냅샷용).</summary>
        private static IEnumerable<int> DownedIds(Func<IEnumerable<Unit>> source)
        {
            foreach (Unit unit in source())
            {
                if (unit != null && IsDowned(unit))
                {
                    yield return unit.gameObject.GetInstanceID();
                }
            }
        }

        /// <summary>유닛이 Downed 행동 상태인지 판정한다.</summary>
        private static bool IsDowned(Unit unit)
        {
            return unit.ActionStateController.CurrentState.ToString() == DownedStateName;
        }

        /// <summary>유닛 표시명(로드아웃 우선, 없으면 GameObject 이름) — matchedCondition 식별 보조.</summary>
        private static string UnitName(Unit unit)
        {
            return unit.UnitLoadOutData != null ? unit.UnitLoadOutData.Unit.DisplayName : unit.name;
        }
    }
}
