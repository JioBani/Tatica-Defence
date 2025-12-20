using Common.Data.Skills.SkillDefinitions;
using Common.Scripts.SceneSingleton;
using Scenes.Battle.Feature.Unit.Skills.Castables;
using Scenes.Battle.Feature.Unit.Skills.Skills;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills
{
    public class SkillFactory : SceneSingleton<SkillFactory>
    {
        public SkillCast CreateSkill(SkillCreateContext context)
        {
            return new FireArrow(context.Attacker);
        } 
    }
}