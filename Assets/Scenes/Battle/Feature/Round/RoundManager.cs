using System;
using System.Collections.Generic;
using Common.Scripts.SceneSingleton;
using Common.Data.Rounds;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Rounds.Phases;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds
{
    
    // TODO: StateBaseController 상속으로 변경
    public class RoundManager : StateBaseController<PhaseType>
    {
        static RoundManager _instance;
        static bool _quitting;
        public int RoundIndex { get; private set; } = 0;
        [SerializeField] private RoundAggressorManager roundAggressorManager;

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
        
        protected override Dictionary<PhaseType, StateBase<PhaseType>> ConfigureStates()
        {
            return new()
            {
                { PhaseType.Maintenance, new MaintenancePhase(this) },
                { PhaseType.Ready, new ReadyPhase(this) },
                { PhaseType.Combat, new CombatPhase(roundAggressorManager, this) },
            };
        }

        public List<RoundInfoData> rounds;

        private void Start()
        {
            StartRound();
        }
        
        public void StartRound()
        {
            RoundIndex++;
            StartStateBase(PhaseType.Maintenance);
        }

        public RoundInfoData GetCurrentRoundData()
        {
            return rounds[RoundIndex];
        }
        
        public void SetReady()
        {
            if (CurrentStateType == PhaseType.Maintenance)
            {
                CurrentState.Exit(PhaseType.Ready);
            }
        }
    }
}


