using Common.Data.Rounds;
using Common.Scripts.StateBase;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class MaintenancePhase : StateBase<PhaseType>
    {
        public MaintenancePhase(
            StateBaseController<PhaseType> controller
        ) : base(
            PhaseType.Maintenance,
            controller
        )
        {
        }

        public override void OnEnter()
        {
            RoundManager.Instance.IncrementRoundIndex();
        }

        public override void OnRun()
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("Maintenance Phase OnExit");
        }

        public override void Dispose()
        {
            
        }
    }
}

