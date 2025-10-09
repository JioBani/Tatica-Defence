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
        private Dictionary<PhaseType, PhaseEvent> _phaseEvents;
        private Dictionary<PhaseType, Phase> _phases;

        private void Awake()
        {
            _phaseEvents = new Dictionary<PhaseType, PhaseEvent>();

            foreach (PhaseType phaseType in (PhaseType[])Enum.GetValues(typeof(PhaseType)))
            {
                _phaseEvents[phaseType] = new PhaseEvent(phaseType);
            }
            
            _phases =  new Dictionary<PhaseType, Phase>();

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

        /// <summary>
        /// 라운드 시작: 원하는 페이즈 타입과 구현을 함께 넘겨주세요.
        /// Enter → (이벤트 발행) → Run → Exit → (이벤트 발행) 순서.
        /// </summary>
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
