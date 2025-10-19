using System;
using System.Collections.Generic;
using Common.Data.Units.UnitStatsByLevel;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Rounds.Unit.ActionState;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Unit
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private ActionStateController actionStateController;
        [SerializeField] private float speed;
        [SerializeField] private Feature.Units.Unit unit;
        
        private Rigidbody2D _rigidbody2D;
        
        private void Awake()
        {
            unit.OnSpawnEvent += SetSpeed;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            var stateBaseEvent = actionStateController.GetStateBase(ActionStateType.Move).Event;
            
            stateBaseEvent.Add(StateBaseEventType.Enter, (_,_) => Move());
            stateBaseEvent.Add(StateBaseEventType.Exit, (_,_) => Stop());
        }

        private void SetSpeed(Feature.Units.Unit unit)
        {
            speed = unit.UnitLoadOutData.Stats.GetStat(UnitStatKind.MoveSpeed, 0);
        }

        private void Move()
        {
            _rigidbody2D.linearVelocity = Vector2.down * speed;
        }

        private void Stop()
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
        }
    }
}
