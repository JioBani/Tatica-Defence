using System;
using System.Collections.Generic;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Rounds.Unit.ActionState.ActionStates;
using Scenes.Battle.Feature.Units.Attacker;
using Scenes.Battle.Feature.Units.Attackable;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Unit.ActionState
{
    public class ActionStateController : StateBaseController<ActionStateType>
    {
        [SerializeField] private Attacker attacker;
        [SerializeField] private bool canMove;
        
        private Rigidbody2D _rigidbody2D;
        
        protected override Dictionary<ActionStateType, StateBase<ActionStateType>> ConfigureStates()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            return new()
            {
                { ActionStateType.Idle, new IdleState(ActionStateType.Idle, attacker)},
                { ActionStateType.Move , new MoveState(ActionStateType.Move, gameObject, attacker)},
                { ActionStateType.Attack , new AttackState(ActionStateType.Attack, gameObject, attacker)},
            };
        }

        private void Start()
        {
            StartStateBase(canMove ? ActionStateType.Move :  ActionStateType.Idle);
        }
    }
}