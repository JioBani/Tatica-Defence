using Common.Data.Rounds;
using Scenes.Battle.Scripts.Unit;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round.Phases
{
    public class CombatPhase : Phase
    {
        private UnitGenerator _unitGenerator;

        public CombatPhase() : base(PhaseType.Combat)
        {
            
        }

        public override void OnEnter()
        {
            Debug.Log("Combat Phase OnEnter");
        }

        public override void OnRun()
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("Combat Phase OnExit");
        }

        public override PhaseType GetNextPhase()
        {
            return PhaseType.Ready;
        }
    }
}