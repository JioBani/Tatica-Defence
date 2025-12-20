using Scenes.Battle.Feature.Unit.Skills.Castables;
using Unity.VisualScripting;

namespace Scenes.Battle.Feature.Unit.Skills.Executables
{
    public abstract class Executable
    {
        private Castable _castable;
        
        protected abstract void Executing();
        
        protected abstract void EndExecuting();
        
        public Executable(Castable castable)
        {
            _castable = castable;
        }

        public void Execute()
        {
            Executing();
            _castable.OnExecute();
        }

        protected void EndExecute()
        {
            EndExecuting();
            _castable.OnExecuteEnd();
        }
    }
} 