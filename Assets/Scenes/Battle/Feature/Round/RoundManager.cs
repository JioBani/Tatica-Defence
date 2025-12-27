using System;
using System.Collections.Generic;
using Common.Scripts.SceneSingleton;
using Common.Data.Rounds;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.LifeCrystals;
using Scenes.Battle.Feature.Rounds.Phases;
using Scenes.Battle.Feature.Unit.Defenders;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds
{

    public class RoundManager : StateBaseController<PhaseType>, IStateListener<PhaseType>
    {
        static RoundManager _instance;
        static bool _quitting;
        public int RoundIndex { get; private set; } = 0;
        [SerializeField] private RoundAggressorManager roundAggressorManager;
        [SerializeField] private DefenderManager defenderManager;
        [SerializeField] private LifeCrystalManager lifeCrystalManager;

        public static RoundManager Instance
        {
            get
            {
                if (_quitting) return null;

                // 이미 캐시되어 있으면 반환
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<RoundManager>(FindObjectsInactive.Exclude);

                return _instance;
            }
        }
        
        public List<RoundInfoData> rounds;

        protected override void Awake()
        {
            base.Awake();

            // IStateListener 등록 (자기 자신)
            RegisterListener(this);
        }

        private void Start()
        {
            StartRound();
        }

        // IStateListener 명시적 구현
        void IStateListener<PhaseType>.OnStateEnter(PhaseType phaseType)
        {
            switch (phaseType)
            {
                case PhaseType.Maintenance:
                    IncrementRoundIndex();
                    break;

                case PhaseType.GameOver:
                    Common.Scripts.GlobalEventBus.GlobalEventBus.Publish(
                        new Scenes.Battle.Feature.Events.RoundEvents.OnGameOverEventDto()
                    );
                    Debug.Log("Game Over");
                    break;
            }
        }

        void IStateListener<PhaseType>.OnStateRun(PhaseType phaseType)
        {
            // Run 단계에서는 특별한 동작 없음
        }

        void IStateListener<PhaseType>.OnStateExit(PhaseType phaseType)
        {
            // Exit 단계에서는 특별한 동작 없음
        }
        
        public void StartRound()
        {
            StartStateBase(PhaseType.Maintenance);
        }

        public RoundInfoData GetCurrentRoundData()
        {
            return rounds[RoundIndex];
        }
        
        protected override PhaseType CheckStateTransition(PhaseType currentPhase)
        {
            // 우선순위 1: 라이프 크리스탈이 파괴되면 무조건 GameOver
            if (currentPhase != PhaseType.GameOver && lifeCrystalManager.IsLifeCrystalDestroyed)
            {
                return PhaseType.GameOver;
            }

            // 우선순위 2: 각 Phase별 전환 조건 체크
            switch (currentPhase)
            {
                case PhaseType.Maintenance:
                    // Maintenance는 SetReady() 호출로만 전환
                    break;

                case PhaseType.Ready:
                    // Ready -> Combat: 자동 전환
                    return PhaseType.Combat;

                case PhaseType.Combat:
                    // Combat -> Maintenance: 모든 적 처치 완료
                    if (roundAggressorManager.IsAllAggressorsCompleted())
                    {
                        return PhaseType.Maintenance;
                    }

                    // Combat -> GameOver: 모든 디펜더 다운
                    if (defenderManager.IsAllDefenderDowned())
                    {
                        return PhaseType.GameOver;
                    }
                    break;

                case PhaseType.GameOver:
                    // GameOver는 전환 없음
                    break;
            }

            return currentPhase;
        }

        public void SetReady()
        {
            if (CurrentStateType == PhaseType.Maintenance)
            {
                RequestStateChange(PhaseType.Ready);
            }
        }

        public void IncrementRoundIndex()
        {
            RoundIndex++;
        }
    }
}


