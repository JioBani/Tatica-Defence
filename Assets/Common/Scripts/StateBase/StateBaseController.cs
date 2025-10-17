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
        private event Action<Dictionary<T, StateBase<T>>> _onConfigureStates;
            
        private void Awake()
        {
            _stateBases = ConfigureStates();
            _onConfigureStates?.Invoke(_stateBases);
            
            foreach (var pair in _stateBases)
            {
                pair.Value.Event.Add(StateBaseEventType.Exit, (_,_) => StartNextState());
            }

            StateBaseAwake();
        }

        protected virtual void StateBaseAwake()
        {
            
        }
        
        public void OnConfigureStatesEvent(Action<Dictionary<T, StateBase<T>>> handler)
        {
            _onConfigureStates += handler;
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