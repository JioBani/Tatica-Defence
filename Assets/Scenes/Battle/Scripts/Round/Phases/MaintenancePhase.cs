using Common.Data.Rounds;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round.Phases
{
    public class MaintenancePhase : Phase
    {
        public MaintenancePhase(PhaseType phaseType) : base(phaseType)
        {
            
        }

        public override void OnEnter()
        {
            
        }

        public override void OnRun()
        {
            Debug.Log("Maintenance Phase OnRun");
        }

        public override void OnExit()
        {
            Debug.Log("Maintenance Phase OnExit");
        }

        public override PhaseType GetNextPhase()
        {
            return PhaseType.Ready;
        }
    }
}

