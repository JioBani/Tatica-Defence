using System;
using System.Collections.Generic;
using Common.Scripts.StateBase;
using Cysharp.Threading.Tasks;
using Scenes.Battle.Scripts.Unit.ActionState.ActionStates;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.ActionState
{
    public class ActionStateController : StateBaseController<ActionStateType>
    {
        protected override Dictionary<ActionStateType, StateBase<ActionStateType>> ConfigureStates()
        {
            return new()
            {
                { ActionStateType.Move , new MoveState(ActionStateType.Move)},
                { ActionStateType.Attack , new AttackState(ActionStateType.Attack)}
            };
        }

        private void Start()
        {
            StartStateBase(ActionStateType.Move);
        }
    }
}