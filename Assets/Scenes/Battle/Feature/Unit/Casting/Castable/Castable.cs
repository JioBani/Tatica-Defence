using System;
using Scenes.Battle.Feature.Unit.Skills.Executables;

namespace Scenes.Battle.Feature.Unit.Skills.Castables
{
    public abstract class Castable
    {
        public Action<Castable, Executable> OnCastEvent;
        public Action<Executable> OnExecuteEvent;
        public Action<Executable> OnExecuteEndEvent;
        private Executable _executable;
        
        public abstract bool CanCast();

        public abstract Executable Casting();

        public void Cast()
        {
            _executable = Casting();
            OnCastEvent?.Invoke(this, _executable);
        }

        public void OnExecute()
        {
            OnExecuteEvent?.Invoke(_executable);
        }

        public void OnExecuteEnd()
        {
            OnExecuteEndEvent?.Invoke(_executable);
        }
    }
}