using Scenes.Battle.Feature.Unit.Skills.Executable;

namespace Scenes.Battle.Feature.Unit.Skills.Castables
{
    public abstract class Castable
    {
        public abstract bool CanCast();

        public abstract IExecutable Cast();
    }
}