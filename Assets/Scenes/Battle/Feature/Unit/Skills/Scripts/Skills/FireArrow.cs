using Scenes.Battle.Feature.Unit.Skills.Contexts;
using Scenes.Battle.Feature.Units.Attackables;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills.SkillExecutors
{
    public class FireArrow : ISkill
    {
        private Units.Unit attacker;
        
        public void Initialize(InitializeContext context)
        {
            attacker = context.Attacker;
        }

        public void Execute(ExecuteContext context)
        {
            Debug.Log("불화살 사용");
        }

        public bool CanExecute(CanExecuteContext context)
        {
            return context.Victim;
        }
    }
}