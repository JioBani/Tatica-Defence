using System;

namespace Common.Scripts.StateBase
{

    /**
     * StateBase 안에는 StateBase의 변화에 대한 내용만 작성되어야 합니다.
     * StateBase 에 따라 동작해야하는 것은 event 를 통해 StateBase 바깥에서 작성되어야 합니다.
     */
    
    public abstract class StateBase<T> where T : Enum
    {
        public T StateType { get; }
        
        public StateBaseEvent<T> Event;
        
        public StateBase(T type)
        {
            StateType = type;
            Event = new StateBaseEvent<T>(type);
        }
    
        public abstract void OnEnter();
        public abstract void OnRun();
        public abstract void OnExit();
        
        public abstract T GetNextStateBaseType();
        
        public void Enter()
        {
            OnEnter();
            Event.Invoke(StateBaseEventType.Enter);
        }

        public void Run()
        {
            OnRun();
        }
        
        public void Exit()
        {
            OnExit();
            Event.Invoke(StateBaseEventType.Exit);
        }
    }
}