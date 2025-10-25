using System;
using System.Collections.Generic;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Units.ActionStates;
using Scenes.Battle.Feature.Units.Attackers;
using Scenes.Battle.Feature.Units.Attackables;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.ActionStates
{
    public class ActionStateController : StateBaseController<ActionStateType>
    {
        [SerializeField] private Unit self;
        [SerializeField] private Attacker attacker;
        [SerializeField] private bool canMove;
        
        private Rigidbody2D _rigidbody2D;
        
        protected override Dictionary<ActionStateType, StateBase<ActionStateType>> ConfigureStates()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            return new()
            {
                { ActionStateType.Idle, new IdleState(ActionStateType.Idle, self, attacker)},
                { ActionStateType.Move , new MoveState(ActionStateType.Move, gameObject, attacker)},
                { ActionStateType.Attack , new AttackState(ActionStateType.Attack, gameObject, attacker)},
                { ActionStateType.Downed , new DownedState(ActionStateType.Downed)},
                { ActionStateType.Freeze , new FreezeState(ActionStateType.Freeze)}
            };
        }

        protected override ActionStateType GlobalTransition(ActionStateType currentStateBaseType)
        {
            if (self.StatSheet.Health.CurrentValue <= 0)
            {
                return ActionStateType.Downed;
            }
            
            return currentStateBaseType;
        }

        private void Start()
        {
            //StartStateBase(canMove ? ActionStateType.Move :  ActionStateType.Idle);
            StartStateBase(canMove ? ActionStateType.Move :  ActionStateType.Freeze);
        }
    }
}