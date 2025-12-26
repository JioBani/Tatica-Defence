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
    
    // TODO: StateBaseController 상속으로 변경
    public class RoundManager : StateBaseController<PhaseType>
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
        
        protected override Dictionary<PhaseType, StateBase<PhaseType>> ConfigureStates()
        {
            return new()
            {
                { PhaseType.Maintenance, new MaintenancePhase(this) },
                { PhaseType.Ready, new ReadyPhase(this) },
                { PhaseType.Combat, new CombatPhase(roundAggressorManager, defenderManager, this) },
                { PhaseType.GameOver, new GameOverPhase(this) }
            };
        }

        public List<RoundInfoData> rounds;

        private void Start()
        {
            StartRound();
        }
        
        public void StartRound()
        {
            StartStateBase(PhaseType.Maintenance);
        }

        public RoundInfoData GetCurrentRoundData()
        {
            return rounds[RoundIndex];
        }
        
        protected override PhaseType GlobalTransition(PhaseType currentStateBaseType)
        {
            if (currentStateBaseType != PhaseType.GameOver && lifeCrystalManager.IsLifeCrystalDestroyed)
            {
                return PhaseType.GameOver;
            }
            return currentStateBaseType;
        }

        
        public void SetReady()
        {
            if (CurrentStateType == PhaseType.Maintenance)
            {
                CurrentState.Exit(PhaseType.Ready);
            }
        }

        public void IncrementRoundIndex()
        {
            RoundIndex++;
        }
    }
}


