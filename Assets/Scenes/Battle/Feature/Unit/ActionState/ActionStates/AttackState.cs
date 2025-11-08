using Common.Scripts.StateBase;
using Cysharp.Threading.Tasks;
using Scenes.Battle.Feature.Units.ActionStates;
using Scenes.Battle.Feature.Units.Attackers;
using Scenes.Battle.Feature.Units.Attackables;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.ActionStates
{
    public class AttackState : StateBase<ActionStateType>
    {
        private Attacker _attacker;
        
        public AttackState(
            ActionStateType type,
            StateBaseController<ActionStateType> controller,
            GameObject self,
            Attacker attacker
        ) : base(type, controller)
        {
            _attacker = attacker;
        }

        public override void OnEnter()
        {
            Debug.Log("AttackState Enter");
        }

        public override void OnRun()
        {
            //TODO: 다운 조건을 더 unit 으로 이동 할 필요가 있을듯
            if (
                !_attacker.Victim ||
                _attacker.Victim.Unit.ActionStateController.CurrentState.StateType == ActionStateType.Downed
            )
            {
                Exit(ActionStateType.Move);
            }
            
            // Debug.Log($"{Event.Events[StateBaseEventType.Enter].Method.Name}");
            // Debug.Log($"{Event.Events[StateBaseEventType.Exit].Method.Name}");
        }

        public override void OnExit()
        {
            Debug.Log("AttackState Exit");
        }
        
        public override void Dispose()
        {
            
        }

        private void DoMove(Victim victim)
        {
            Exit(ActionStateType.Move);
        }
    }
}