using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round
{
    public class RoundManager : MonoBehaviour
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
        }

        private void Start()
        {
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
        public void StartRound(PhaseType phaseType, Phase phase)
        {
            RoundIndex++;
            
            
        }

        public void StartPhase(PhaseType phaseType)
        {
            _currentPhase = _phases[phaseType];

            _currentPhase.Enter();
        }

        /// <summary>
        /// Action 콜백 등록. (phaseType, eventType) 컨텍스트를 함께 전달.
        /// </summary>
        public void AddAction(PhaseType phaseType, PhaseEventType phaseEvent, Action<PhaseType, PhaseEventType> action)
        {
            _phaseEvents[phaseType].Add(phaseEvent, action);
        }

        public void RemoveAction(PhaseType phaseType, PhaseEventType phaseEvent, Action<PhaseType, PhaseEventType> action)
        {
            _phaseEvents[phaseType].Remove(phaseEvent, action);
        }
    }
}
