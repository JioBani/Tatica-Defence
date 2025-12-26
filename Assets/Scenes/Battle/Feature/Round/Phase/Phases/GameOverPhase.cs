using Common.Scripts.GlobalEventBus;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Events.RoundEvents;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class GameOverPhase : StateBase<PhaseType>
    {

        public GameOverPhase(
            StateBaseController<PhaseType> controller
        ) : base(PhaseType.GameOver, controller)
        {

        }

        public override void OnEnter()
        {
            GlobalEventBus.Publish<OnGameOverEventDto>(new OnGameOverEventDto());
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