using Common.Data.Skills.SkillDefinitions;

namespace Scenes.Battle.Feature.Unit.Skills.Contexts
{
    public class InitializeContext
    {
        public Units.Unit Attacker { get; private set; }

        public InitializeContext(Units.Unit attacker)
        {
            Attacker = attacker;
        }
    }
}   