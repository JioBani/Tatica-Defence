using JetBrains.Annotations;

namespace Scenes.Battle.Feature.Unit.Skills.Contexts
{
    public class SkillRuntimeContext
    {
        public readonly Units.Unit Attacker;
        [CanBeNull] public readonly Units.Unit Victim;

        public SkillRuntimeContext(Units.Unit attacker, [CanBeNull] Units.Unit victim)
        {
            Attacker = attacker;
            Victim = victim;
        }
    }
}