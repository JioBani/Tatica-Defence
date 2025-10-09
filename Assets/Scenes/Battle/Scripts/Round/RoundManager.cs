using System;
using System.Collections.Generic;
using Common.SceneSingleton;
using Scenes.Battle.Scripts.Round.Phases;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round
{
    public class RoundManager : SceneSingleton<RoundManager>
    {
        public int RoundIndex { get; private set; } = 0;
        private Phase _currentPhase; 
        private Dictionary<PhaseType, Phase> _phases;

        protected override void OnAwakeSingleton()
        {
            base.OnAwakeSingleton();
            
            _phases = new()
            {
                { PhaseType.Maintenance, new MaintenancePhase(PhaseType.Maintenance) }
            };
            
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
    }
}
