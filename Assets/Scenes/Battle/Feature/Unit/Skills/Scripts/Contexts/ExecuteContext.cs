using JetBrains.Annotations;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Skills.Contexts
{
    public class ExecuteContext
    {
        public readonly Attacker Attacker;
        [CanBeNull] public readonly Victim Victim;

        public ExecuteContext(Attacker attacker, [CanBeNull] Victim victim)
        {
            Attacker = attacker;
            Victim = victim;
        }
    }
}