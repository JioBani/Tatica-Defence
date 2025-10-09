using UnityEngine;

namespace Scenes.Battle.Scripts.Round.Phases
{
    public class Maintenance : Phase
    {
        public PhaseType PhaseType { get; } = PhaseType.Maintenance;
        public override void OnEnter()
        {
            
        }

        public override void OnRun()
        {
        }

        public override void OnExit()
        {
        }

        protected override PhaseType GetNextPhase()
        {
            return PhaseType.Ready;
        }
    }
}

