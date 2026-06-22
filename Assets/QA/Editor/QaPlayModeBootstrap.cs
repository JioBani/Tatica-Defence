using UnityEditor;
using UnityEngine;

namespace QA.Editor
{
    // ─────────────────────────────────────────────
    // QaPlayModeBootstrap: 에디터 Play 진입마다 QA 자동화에 필요한 런타임 환경을 보장하는 에디터 전용 훅.
    //   - 목적: "환경 보장이 수동"(P4) 제거 — 매 세션 execute_code 로 runInBackground 를 주입하던 절차를 자동화.
    //   - 비포커스 에디터에서도 플레이모드가 계속 틱하도록 Application.runInBackground 를 켠다
    //     (안 켜면 에디터 비포커스 시 게임 루프가 멈춰 QA 시나리오 진행 불가).
    //   - 격리: 에디터 전용([InitializeOnLoad], Assembly-CSharp-Editor)이며 PlayerSettings/빌드 설정은 건드리지 않는다.
    //     Application.runInBackground 는 Play 세션 한정 런타임 오버라이드라 플레이어 빌드 동작·게임 코드에 영향 0.
    //     QA→게임 단방향 유지(게임은 QA 를 모른다).
    //   - timeScale/isPaused 는 시나리오가 의도적으로 조작할 수 있어 강제하지 않는다(여기서는 runInBackground 만 보장).
    // ─────────────────────────────────────────────
    [InitializeOnLoad]
    public static class QaPlayModeBootstrap
    {
        /// <summary>에디터 로드 시 Play 모드 상태 변화 구독을 등록한다.</summary>
        static QaPlayModeBootstrap()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>Play 진입 시 QA 런타임 환경을 보장한다.</summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                EnsureRunnable();
            }
        }

        /// <summary>비포커스 에디터에서도 플레이모드가 계속 틱하도록 보장한다.</summary>
        private static void EnsureRunnable()
        {
            // 이미 켜져 있으면(PlayerSettings 가 true 이거나 이전 보장) 로그 없이 통과한다.
            if (!Application.runInBackground)
            {
                Application.runInBackground = true;
                Debug.Log("[QA] runInBackground=true 보장(비포커스 에디터에서도 플레이모드 틱 유지).");
            }
        }
    }
}
