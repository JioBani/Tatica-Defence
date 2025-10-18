using System;
using System.Collections.Generic;
using Common.Scripts.StateBase;
using Cysharp.Threading.Tasks;
using Scenes.Battle.Scripts.Unit.ActionState.ActionStates;
using Scenes.Battle.Scripts.Unit.Attackable;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Battle.Scripts.Unit.ActionState
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