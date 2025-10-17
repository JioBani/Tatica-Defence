using Common.Data.Units.UnitStatsByLevel;
using Common.Scripts.StateBase;
using Scenes.Battle.Scripts.Unit.ActionState;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private ActionStateController actionStateController;
        [SerializeField] private float speed;
        [SerializeField] private Unit unit;
        
        private Rigidbody2D _rigidbody2D;
        
        private void Awake()
        {
            unit.onSpawnEvent += SetSpeed;
            _rigidbody2D = GetComponent<Rigidbody2D>();

            actionStateController.OnConfigureStatesEvent((states) =>
            {
                var stateBaseEvent = states[ActionStateType.Move].Event;
                
                stateBaseEvent.Add(StateBaseEventType.Enter, (_,_) => Move());
                stateBaseEvent.Add(StateBaseEventType.Exit, (_,_) => Stop());
            });
        }

        private void SetSpeed(Unit unit)
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
