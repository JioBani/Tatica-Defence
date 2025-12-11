using JetBrains.Annotations;
using Scenes.Battle.Feature.Units.Attackables;

namespace Scenes.Battle.Feature.Unit.Skills.Contexts
{
    public class ExecuteContext
    {
        [CanBeNull] public readonly Victim Victim;

        public ExecuteContext([CanBeNull] Victim victim)
        {
            Victim = victim;
        }
    }
}