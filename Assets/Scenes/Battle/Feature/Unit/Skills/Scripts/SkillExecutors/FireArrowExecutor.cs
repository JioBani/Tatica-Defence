using Scenes.Battle.Feature.Unit.Skills.Contexts;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills.SkillExecutors
{
    public class FireArrowExecutor : ISkillExecutor
    {
        public void Initialize(SkillInitializeContext context)
        {
            
        }

        public void Execute(SkillRuntimeContext context)
        {
            Debug.Log("불화살 사용");
        }
    }
}