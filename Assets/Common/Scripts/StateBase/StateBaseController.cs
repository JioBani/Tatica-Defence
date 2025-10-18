using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Scripts.StateBase
{
    public abstract class StateBaseController<T> : MonoBehaviour where T : Enum
    {
        protected Dictionary<T, StateBase<T>> _stateBases;
        
        private StateBase<T> _currentState;
        
        protected abstract Dictionary<T, StateBase<T>> ConfigureStates();
        public event Action<Dictionary<T, StateBase<T>>> OnConfigureStatesEvent;
            
        private void Awake()
        {
            _stateBases = ConfigureStates();
            OnConfigureStatesEvent?.Invoke(_stateBases);
            
            foreach (var pair in _stateBases)
            {
                pair.Value.Event.Add(StateBaseEventType.Exit, (_,_) => StartNextState());
            }

            StateBaseAwake();
        }

        private void Update()
        {
            _currentState.Run();
        }

        protected virtual void StateBaseAwake()
        {
            
        }
        
        public void StartStateBase(T type)
        {
            _currentState = _stateBases[type];
            _currentState.Enter();
        }

        private void StartNextState()
        {
            _currentState = _stateBases[_currentState.GetNextStateBaseType()];
            
            _currentState.Enter();
        }

        public StateBase<T> GetCurrentState()
        {
            return _currentState;
        }

        public StateBase<T> GetStateBase(T type)
        {
            return _stateBases[type];
        }
    }
}