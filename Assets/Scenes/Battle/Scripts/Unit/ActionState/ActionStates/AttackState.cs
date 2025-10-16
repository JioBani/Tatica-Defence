using Common.Scripts.StateBase;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.ActionState.ActionStates
{
    public class AttackState : StateBase<ActionStateType>
    {
        public AttackState(ActionStateType type) : base(type)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("AttackState Enter");
            
            DoNextState();
        }

        public override void OnRun()
        {
            Debug.Log("AttackState Run");
        }

        public override void OnExit()
        {
            Debug.Log("AttackState Exit");
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