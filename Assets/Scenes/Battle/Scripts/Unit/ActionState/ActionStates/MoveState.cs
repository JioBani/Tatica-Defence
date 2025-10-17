using Common.Scripts.StateBase;
using Scenes.Battle.Scripts.Unit.Attackable;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.ActionState.ActionStates
{
    public class MoveState : StateBase<ActionStateType>
    {
        private GameObject _self;
        private Attacker _attacker;
        
        public MoveState(
            ActionStateType type, 
            GameObject self,
            Attacker attacker
        ) : base(type)
        {
            _self = self;
            _attacker = attacker;
            _attacker.OnTargetEnter += DoAttack;
        }

        public override void OnEnter()
        {
            Debug.Log("MoveState Enter");
        }

        public override void OnRun()
        {
            Debug.Log("MoveState Run");
        }

        public override void OnExit()
        {
            
        }

        public override ActionStateType GetNextStateBaseType()
        {
            Debug.Log("GetNextStateBase");
            return ActionStateType.Attack;
        }

        private void DoAttack(Victim victim)
        {
            Exit();
        }
    }
}