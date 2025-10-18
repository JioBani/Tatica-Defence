using Common.Scripts.StateBase;
using Scenes.Battle.Scripts.Unit.Attackable;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.ActionState.ActionStates
{
    public class IdleState : StateBase<ActionStateType>
    {
        public IdleState(
            ActionStateType type,
            Attacker attacker
        ) : base(type)
        {
            attacker.OnTargetEnter += DoAttack;
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

        public override ActionStateType GetNextStateBaseType()
        {
            return ActionStateType.Attack;
        }
        
        private void DoAttack(Victim victim)
        {
            Exit();
        }
    }
}
