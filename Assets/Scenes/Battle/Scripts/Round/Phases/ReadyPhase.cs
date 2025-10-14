using Common.Data.Rounds;
using Scenes.Battle.Scripts.Unit;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round.Phases
{
    public class ReadyPhase : Phase
    {
        
        public ReadyPhase() : base(PhaseType.Ready)
        {
            
        }

        public override void OnEnter()
        {
            Exit();
        }

        public override void OnRun()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override PhaseType GetNextPhase()
        {
            return PhaseType.Combat;
        }
    }
}