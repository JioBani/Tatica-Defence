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
            //TODO: 라운드 정보 표시 함수 호출
            Debug.Log("Maintenance Phase OnEnter");
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

