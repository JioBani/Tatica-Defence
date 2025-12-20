using Scenes.Battle.Feature.Unit.Skills.Castables;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills.Executables
{
    public class FireArrowExecutor : Executable
    {
        private Attacker _attacker;
        private Victim _victim;
        
        public FireArrowExecutor(Castable castable, Attacker attacker, Victim victim) : base(castable)
        {
            _attacker = attacker;
            _victim = victim;
        }

        protected override void Executing()
        {
            Debug.Log("Executing FireArrow");
            EndExecute();
        }

        protected override void EndExecuting()
        {
            
        }
    }
}