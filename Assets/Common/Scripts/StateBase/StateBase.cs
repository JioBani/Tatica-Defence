using System;

namespace Common.Scripts.StateBase
{
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
        
        public abstract T GetNextStateBase();
        
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