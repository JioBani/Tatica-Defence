using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Scripts.StateBase
{
    public abstract class StateBaseController<T> : MonoBehaviour where T : struct, Enum
    {
        protected Dictionary<T, StateBase<T>> _stateBases;
        
        [CanBeNull] private StateBase<T> _currentState;
        [CanBeNull] public StateBase<T> CurrentState => _currentState;
        public T? CurrentStateType => _currentState?.StateType;
        
        [SerializeField] private T showState; // 인스펙터 노출용
        
        
        protected abstract Dictionary<T, StateBase<T>> ConfigureStates();
        public event Action<Dictionary<T, StateBase<T>>> OnConfigureStatesEvent;
            
        protected virtual void Awake()
        {
            _stateBases = ConfigureStates();
            OnConfigureStatesEvent?.Invoke(_stateBases);
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

        public void Exit(T nextState)
        {
            _currentState = _stateBases[nextState];
            
            _currentState.Enter();
        }

        /// <summary>
        /// 해당 StateBaseController에서 관리되는 모든 StateBase 에 대해서
        /// 공통적인 상태 변화를 지시
        /// </summary>
        /// <param name="currentStateBase"></param>
        /// <returns></returns>
        protected virtual T GlobalTransition(T currentStateBase)
        {
            return currentStateBase;
        }
    }
}