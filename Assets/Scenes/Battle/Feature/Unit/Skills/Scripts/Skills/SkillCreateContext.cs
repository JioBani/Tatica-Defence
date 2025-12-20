using Common.Data.Skills.SkillDefinitions;
using JetBrains.Annotations;
using Scenes.Battle.Feature.Unit.Castables;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Skills
{
    public class SkillCreateContext
    {
        public readonly SkillDefinitionData Data;
        public readonly SkillCaster Caster;
        [CanBeNull] public readonly Attacker Attacker;

        public SkillCreateContext(SkillDefinitionData data,SkillCaster caster, [CanBeNull] Attacker attacker)
        {
            Data = data;
            Attacker = attacker;
            Caster = caster;
        }
    }
}