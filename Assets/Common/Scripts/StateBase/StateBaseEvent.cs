using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Scripts.StateBase
{
    public class StateBaseEvent<T> where T : Enum
    {
        private readonly T _stateBaseType;

        private readonly Dictionary<StateBaseEventType, Action<T, StateBaseEventType>> _events;
        
        public Dictionary<StateBaseEventType, Action<T, StateBaseEventType>> Events => _events;
        
        private static readonly StateBaseEventType[] AllEventTypes =
            (StateBaseEventType[])Enum.GetValues(typeof(StateBaseEventType));
     
        public StateBaseEvent(T stateBaseType)
        {
            _stateBaseType = stateBaseType;
            
            _events = new Dictionary<StateBaseEventType, Action<T, StateBaseEventType>>(AllEventTypes.Length);
            
            foreach (var type in AllEventTypes)
            {
                _events[type] = null;
            }
        }
        
        public void Add(StateBaseEventType eventType, Action<T, StateBaseEventType> action)
        {
            _events[eventType] += action;
        }

        public void Remove(StateBaseEventType eventType, Action<T, StateBaseEventType> action)
        {
            _events[eventType] -= action;
        }

        public void Invoke(StateBaseEventType eventType)
        {
            _events[eventType]?.Invoke(_stateBaseType, eventType);
        }
    }
}