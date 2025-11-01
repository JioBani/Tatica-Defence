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
    public class RoundManager : SceneSingleton<RoundManager>
    {
        public int RoundIndex { get; private set; } = 0;
        private Phase _currentPhase;
        [SerializeField] private RoundAggressorManager roundAggressorManager;

        private readonly Dictionary<PhaseType, Phase> _phases = new()
        {
            { PhaseType.Maintenance, new MaintenancePhase() },
            { PhaseType.Ready, new ReadyPhase() },
            { PhaseType.Combat, new CombatPhase() },
        };

        public List<RoundInfoData> rounds;

        protected override void OnAwakeSingleton()
        {
            base.OnAwakeSingleton();
            
            // 한 페이즈 종료시 다음 페이즈 호출하는 콜백 등록
            foreach (var pair in _phases)
            {
                pair.Value.phaseEvent.Add(PhaseEventType.Exit, (_,_) => StartNextPhase());
            }
        }

        private void Start()
        {
            StartRound();
        }

        private void Update()
        {
            if (_currentPhase is not null)
            {
                _currentPhase.Run();
                
                // 현재 페이즈가 Combat 이고, 모든 침략자가 소환 되고 처치 되었으면 전투 페이즈 종료
                if (
                    _currentPhase.PhaseType == PhaseType.Combat && 
                    roundAggressorManager.IsAllAggressorsCompleted()
                    )
                {
                    _currentPhase.Exit();
                }
            }
        }
        
        public void StartRound()
        {
            RoundIndex++;
            
            _currentPhase = _phases[PhaseType.Maintenance];
            _currentPhase.Enter();
        }

        private void StartNextPhase()
        {
            _currentPhase = _phases[_currentPhase.GetNextPhase()];
            
            _currentPhase.Enter();
        }

        public RoundInfoData GetCurrentRoundData()
        {
            return rounds[RoundIndex];
        }

        public Phase GetPhase(PhaseType type)
        {
            return _phases[type];
        }

        public void SetReady()
        {
            if (_currentPhase.PhaseType == PhaseType.Maintenance)
            {
                GetPhase(PhaseType.Maintenance).Exit();
            }
        }
    }
}


