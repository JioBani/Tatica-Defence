using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace QA.Editor
{
    // ─────────────────────────────────────────────
    // QaWaitEngine: 조건 충족 또는 타임아웃까지 매 프레임 폴링하는 블로킹 대기 엔진(T8 RunUntil 토대).
    //   - qa_await(async hold-open)에서만 구동 — while 폴링을 await UniTask.Yield(Update)로 매프레임 양보(동기 블로킹 금지).
    //   - 타임아웃은 realtime(timeScale 무관) 기준 — freeze/감속 중에도 상한이 흐르게 해 무한대기를 막는다. 단일 hold ≤28s 캡.
    //   - 조건 평가는 주입받은 probe(Func<object>, QaConditionFactory 생성)에 위임한다 — 엔진은 조건 종류를 모른다(개방-폐쇄).
    //     probe()가 non-null 을 반환하면 충족(반환값=matchedCondition), null 이면 미충족.
    //   - 충족 시 freezeOnTrigger 면 시뮬레이션을 정지(timeScale=0) — Pause 와 동일 메커니즘 재사용(요구사항: 정지 메커니즘 일관).
    // RunUntil 조건셋: QaApi.RunUntil 이 QaConditionFactory.Build 로 probe 를 만들어 이 엔진에 넘긴다.
    // ─────────────────────────────────────────────
    internal static class QaWaitEngine
    {
        /// <summary>단일 hold 상한(ms). 벤더 브릿지 30s 캡 마진.</summary>
        private const int MaxHoldMs = 28000;

        /// <summary>대기 종료 결과: 조건 충족 여부·매치 정보·경과(ms)·정지 적용 여부.</summary>
        internal readonly struct WaitOutcome
        {
            /// <summary>조건이 충족돼 종료했으면 true, 타임아웃이면 false.</summary>
            public readonly bool Triggered;

            /// <summary>충족 시 probe 가 반환한 매치 정보(matchedCondition). 타임아웃이면 null.</summary>
            public readonly object Match;

            /// <summary>대기에 실제 소요된 시간(ms, realtime).</summary>
            public readonly double ElapsedMs;

            /// <summary>충족 시 정지(timeScale=0)가 적용됐으면 true.</summary>
            public readonly bool Frozen;

            public WaitOutcome(bool triggered, object match, double elapsedMs, bool frozen)
            {
                Triggered = triggered;
                Match = match;
                ElapsedMs = elapsedMs;
                Frozen = frozen;
            }
        }

        /// <summary>probe 가 non-null 을 반환하거나 timeoutMs 경과까지 매 프레임 폴링한다. 충족 시 freezeOnTrigger 면 정지시킨다.</summary>
        internal static async Task<WaitOutcome> WaitUntil(Func<object> probe, int timeoutMs, bool freezeOnTrigger)
        {
            if (probe == null)
            {
                throw new ArgumentNullException(nameof(probe));
            }

            // 단일 hold 상한으로 캡(호출자가 더 큰 값을 줘도 브릿지 캡 마진 안으로 자른다).
            int cappedTimeout = Mathf.Clamp(timeoutMs, 0, MaxHoldMs);
            if (timeoutMs > MaxHoldMs)
            {
                Debug.LogWarning($"[QA] RunUntil timeout {timeoutMs}ms 가 상한 {MaxHoldMs}ms 초과 — {MaxHoldMs}ms 로 제한합니다.");
            }

            // 타임아웃은 realtime 기준(timeScale 무관) — freeze/감속 중에도 상한이 흐른다.
            float startRealtime = Time.realtimeSinceStartup;
            object match = null;
            while (true)
            {
                match = probe();
                if (match != null)
                {
                    // 충족 — 매치 정보를 들고 종료.
                    break;
                }

                double elapsed = (Time.realtimeSinceStartup - startRealtime) * 1000.0;
                if (elapsed >= cappedTimeout)
                {
                    // 타임아웃 — 미충족 종료.
                    break;
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            bool triggered = match != null;
            bool frozen = false;
            if (triggered && freezeOnTrigger)
            {
                // 충족 프레임에 정지 — Pause 와 동일 메커니즘(timeScale=0)을 재사용한다.
                Time.timeScale = 0f;
                frozen = true;
            }

            double totalMs = (Time.realtimeSinceStartup - startRealtime) * 1000.0;
            return new WaitOutcome(triggered, match, totalMs, frozen);
        }
    }
}
