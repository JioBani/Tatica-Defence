using Common.Scripts.StateBase;
using Cysharp.Threading.Tasks;
using Scenes.Battle.Feature.Units.Attackers;
using Scenes.Battle.Feature.Units.Attackable;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Unit.ActionState.ActionStates
{
    public class AttackState : StateBase<ActionStateType>
    {
        private Attacker _attacker;
        
        public AttackState(
            ActionStateType type, 
            GameObject self,
            Attacker attacker
        ) : base(type)
        {
            _attacker = attacker;
            _attacker.OnTargetExit += DoMove;
        }

        public override void OnEnter()
        {
            Debug.Log("AttackState Enter");
        }

        public override void OnRun()
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("AttackState Exit");
        }

        public override ActionStateType GetNextStateBaseType()
        {
            Debug.Log("GetNextStateBase");
            return ActionStateType.Attack;
        }

        private void DoMove(Victim victim)
        {
            Exit();
        }
    }
}