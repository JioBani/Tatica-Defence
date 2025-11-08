using Common.Scripts.StateBase;

namespace Scenes.Battle.Feature.Units.ActionStates
{
    public class FreezeState : StateBase<ActionStateType>
    {
        public FreezeState(
            ActionStateType type,
            StateBaseController<ActionStateType> controller
        ) : base(
            type,
            controller
        )
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