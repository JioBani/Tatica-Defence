using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Units.Attackers;
using Scenes.Battle.Feature.Units.Attackables;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.ActionStates
{
    public class MoveState : StateBase<ActionStateType>
    {
        private GameObject _self;
        private readonly Attacker _attacker;
        
        public MoveState(
            ActionStateType type, 
            StateBaseController<ActionStateType> controller,
            GameObject self,
            Attacker attacker
        ) : base(type, controller)
        {
            _self = self;
            _attacker = attacker;
        }

        public override void OnEnter()
        {
        }

        public override void OnRun()
        {
            if (_attacker.Victim)
            {
                Exit(ActionStateType.Attack);
            }
        }

        public override void OnExit()
        {
            
        }

        public override void Dispose()
        {
           //_attacker.OnTargetEnter -= DoAttack;
        }

        private void DoAttack(Victim victim)
        {
            Exit(ActionStateType.Attack);
        }
    }
}