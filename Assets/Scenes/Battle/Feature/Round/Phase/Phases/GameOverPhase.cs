using Common.Scripts.StateBase;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class GameOverPhase : StateBase<PhaseType>
    {

        public GameOverPhase(
            StateBaseController<PhaseType> controller
        ) : base(PhaseType.Ready, controller)
        {

        }

        public override void OnEnter()
        {
            Debug.Log("Game Over");
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