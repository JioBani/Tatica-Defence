using System;

namespace Common.Scripts.StateBase
{
    /**
     * StateBase 안에는 StateBase의 변화에 대한 내용만 작성되어야 합니다.
     * StateBase 에 따라 동작해야하는 것은 event 를 통해 StateBase 바깥에서 작성되어야 합니다.
     */
    
    public abstract class StateBase<T> : IDisposable where T : struct, Enum
    {
        public T StateType { get; }
        
        public StateBaseEvent<T> Event;
        
        private StateBaseController<T> _controller;
        
        public StateBase(T type, StateBaseController<T> controller)
        {
            StateType = type;
            _controller = controller;
            Event = new StateBaseEvent<T>(type);
        }
    
        public abstract void OnEnter();
        public abstract void OnRun();
        public abstract void OnExit();
        
        public void Enter()
        {
            OnEnter();
            Event?.Invoke(StateBaseEventType.Enter);
        }

        public void Run()
        {
            OnRun();
            Event?.Invoke(StateBaseEventType.Run);
        }
        
        public void Exit(T nextStateBaseType)
        {
            OnExit();
            Event?.Invoke(StateBaseEventType.Exit);
            _controller.Exit(nextStateBaseType);
        }

        public abstract void Dispose();
    }
}