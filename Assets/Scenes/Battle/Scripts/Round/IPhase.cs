using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round
{
    public abstract class Phase
    {
        PhaseType PhaseType { get; }
        public abstract void OnEnter();
        public abstract void OnRun();
        public abstract void OnExit();
        protected abstract PhaseType GetNextPhase();

        public void Enter()
        {
            OnEnter();
        }

        public void Run()
        {
            OnRun();
        }
        
        public PhaseType Exit()
        {
            OnExit();

            return GetNextPhase();
        }
    }
}
