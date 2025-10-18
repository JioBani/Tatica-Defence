using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds
{
    public abstract class Phase
    {
        public PhaseType PhaseType { get; private set; }
        public PhaseEvent phaseEvent;

        public Phase(PhaseType phaseType)
        {
            PhaseType = phaseType;
            
            phaseEvent = new PhaseEvent(phaseType);
        }
        
        public abstract void OnEnter();
        public abstract void OnRun();
        public abstract void OnExit();
        public abstract PhaseType GetNextPhase();

        public void Enter()
        {
            OnEnter();
            phaseEvent.Invoke(PhaseEventType.Enter);
        }

        public void Run()
        {
            OnRun();
        }
        
        public PhaseType Exit()
        {
            OnExit();
            phaseEvent.Invoke(PhaseEventType.Exit);

            return GetNextPhase();
        }
    }
}
