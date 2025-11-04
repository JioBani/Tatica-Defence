using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Units.ActionStates;
using Scenes.Battle.Feature.Units.Attackers;
using Scenes.Battle.Feature.Units.Attackables;

namespace Scenes.Battle.Feature.Units.ActionStates
{
    public class DownedState : StateBase<ActionStateType>
    {
        
        public DownedState(
            ActionStateType type,
            StateBaseController<ActionStateType> controller
        ) : base(type, controller)
        {
        }

        public override void OnEnter()
        {
            
        }

        public override void OnRun()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void Dispose()
        {
        }
    }
}