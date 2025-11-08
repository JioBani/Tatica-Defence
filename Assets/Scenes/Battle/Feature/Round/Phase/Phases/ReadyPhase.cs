using Common.Scripts.StateBase;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class ReadyPhase : StateBase<PhaseType>
    {

        public ReadyPhase(
            StateBaseController<PhaseType> controller
        ) : base(PhaseType.Ready, controller)
        {

        }

        public override void OnEnter()
        {
            Exit(PhaseType.Combat);
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