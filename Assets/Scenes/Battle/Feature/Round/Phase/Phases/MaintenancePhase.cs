using Common.Data.Rounds;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class MaintenancePhase : Phase
    {
        public MaintenancePhase() : base(PhaseType.Maintenance)
        {
            
        }

        public override void OnEnter()
        {
            
        }

        public override void OnRun()
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("Maintenance Phase OnExit");
        }

        public override PhaseType GetNextPhase()
        {
            return PhaseType.Ready;
        }

        /// <summary>
        /// 전투 준비 완료
        /// </summary>
        public void SetReady()
        {
            Exit();
        }
    }
}

