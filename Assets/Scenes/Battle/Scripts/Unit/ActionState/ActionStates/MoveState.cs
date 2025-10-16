using Common.Scripts.StateBase;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.ActionState.ActionStates
{
    public class MoveState : StateBase<ActionStateType>
    {
        public MoveState(ActionStateType type) : base(type)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("MoveState Enter");

            DoNextState();
        }

        public override void OnRun()
        {
            Debug.Log("MoveState Run");
        }

        public override void OnExit()
        {
            Debug.Log("MoveState Exit");
        }

        public override ActionStateType GetNextStateBase()
        {
            Debug.Log("GetNextStateBase");
            return ActionStateType.Attack;
        }

        private async UniTask DoNextState()
        {
            await UniTask.Delay(500);

            Exit();
        }
    }
}