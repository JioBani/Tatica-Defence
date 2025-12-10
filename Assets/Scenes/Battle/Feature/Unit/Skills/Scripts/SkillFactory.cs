using Common.Data.Skills.SkillDefinitions;
using Common.Scripts.SceneSingleton;
using Scenes.Battle.Feature.Unit.Skills.SkillExecutors;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills
{
    public class SkillFactory : SceneSingleton<SkillFactory>
    {
        public ISkillExecutor CreateSkillExecutor(SkillDefinitionData skillData)
        {
            return new FireArrowExecutor();
        } 
    }
}