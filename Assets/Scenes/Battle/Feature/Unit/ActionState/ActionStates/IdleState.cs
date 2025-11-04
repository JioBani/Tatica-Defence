using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Units.Attackers;
using Scenes.Battle.Feature.Units.Attackables;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.ActionStates
{
    public class IdleState : StateBase<ActionStateType>
    {
        private Unit _self;
        private Attacker _attacker;
        
        public IdleState(
            ActionStateType type,
            StateBaseController<ActionStateType> controller,
            Unit self,
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
            
        }
        
        private void DoAttack(Victim victim)
        {
            Exit(ActionStateType.Attack);
        }
    }
}
