using Common.Scripts.ObjectPool;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Units.ActionStates;
using UnityEngine;

namespace Scenes.Battle.Feature.Aggressors
{
    public class Aggressor : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2d;

        [SerializeField] private Poolable poolable;
        [SerializeField] private ActionStateController actionStateController;

        private void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            actionStateController
                .GetStateBase(ActionStateType.Downed)
                .Event
                .Add(
                    StateBaseEventType.Enter,
                    (_, _) => { OnDown(); }
                );
            _rigidbody2d.linearVelocity = Vector2.down * 1f;
        }

        private void OnDisable()
        {
            actionStateController
                .GetStateBase(ActionStateType.Downed)
                .Event
                .Remove(
                    StateBaseEventType.Enter,
                    (_, _) => { OnDown(); }
                );

            _rigidbody2d.linearVelocity = Vector2.zero;
        }

        private void OnDown()
        {
            poolable.DeSpawn();
        }

        /// <summary>
        /// 생명 수정 위험 지역에 진입한 경우
        /// </summary>
        public void OnEnterLifeCrystalContactZone()
        {
            poolable.DeSpawn();
        }
    }
}
