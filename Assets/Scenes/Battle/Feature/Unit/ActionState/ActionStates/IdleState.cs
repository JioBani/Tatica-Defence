using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Units.Attacker;
using Scenes.Battle.Feature.Units.Attackable;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Unit.ActionState.ActionStates
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
