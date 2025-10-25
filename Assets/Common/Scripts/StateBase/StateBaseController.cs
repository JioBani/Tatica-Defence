using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Scripts.StateBase
{
    public abstract class StateBaseController<T> : MonoBehaviour where T : Enum
    {
        protected Dictionary<T, StateBase<T>> _stateBases;
        
        private StateBase<T> _currentState;
        public StateBase<T> CurrentState => _currentState;
        
        [SerializeField] private T showState; // 인스펙터 노출용
        
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
            showState = _currentState.StateType;
            _currentState.Run();
            
            T nextState = GlobalTransition(_currentState.StateType);

            if (!nextState.Equals(_currentState.StateType))
            {
                _currentState.Exit(nextState);
            }
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

        private void OnDestroy()
        {
            foreach (var pair in _stateBases)
            {
                pair.Value.Dispose();
            }
        }

        protected virtual T GlobalTransition(T currentStateBase)
        {
            return currentStateBase;
        }
    }
}