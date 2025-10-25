using System;
using System.Collections.Generic;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class PhaseEvent
    {
        private readonly PhaseType _phaseType;

        // 각 이벤트 타입별로 Action 체인을 보관
        private readonly Dictionary<PhaseEventType, Action<PhaseType, PhaseEventType>> events;

        public PhaseEvent(PhaseType phaseType)
        {
            this._phaseType = phaseType;

            events = new Dictionary<PhaseEventType, Action<PhaseType, PhaseEventType>>();

            foreach (PhaseEventType eventType in (PhaseEventType[])Enum.GetValues(typeof(PhaseEventType)))
            {
                events[eventType] = null; // 아직 구독 없으면 null
            }
        }

        public void Add(PhaseEventType eventType, Action<PhaseType, PhaseEventType> action)
        {
            events[eventType] += action;
        }

        public void Remove(PhaseEventType eventType, Action<PhaseType, PhaseEventType> action)
        {
            events[eventType] -= action;
        }

        public void Invoke(PhaseEventType eventType)
        {
            events[eventType]?.Invoke(_phaseType, eventType);
        }
    }
}