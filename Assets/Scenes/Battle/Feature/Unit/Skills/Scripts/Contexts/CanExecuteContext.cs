using JetBrains.Annotations;
using Scenes.Battle.Feature.Units.Attackables;

namespace Scenes.Battle.Feature.Unit.Skills.Contexts
{
    public class CanExecuteContext
    {
        public readonly Victim Victim;

        public CanExecuteContext([CanBeNull] Victim victim)
        {
            Victim = victim;
        }
    }
}