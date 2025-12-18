using System;
using Common.Scripts.ObjectPool;
using Scenes.Battle.Feature.Units.Attackers;
using UnityEngine;

namespace Scenes.Battle.Feature.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public event Action OnHit;

        [SerializeField] private Poolable poolable;
        [SerializeField] private float speed;     // 이동 속도 (units/sec)
        [SerializeField] private float hitRadius;// 타겟에 도달했다고 보는 거리
        [SerializeField] private float maxLifetime;// 안전 타이머

        private Transform _target;
        private float _life;

        // TODO: attacker 만을 위한 것이 아닌, 투사체 전체로 확장
        // public void Shot(Attacker attacker, Transform target)
        // {
        //     _target = target;
        //     _life = 0f;
        //
        //     transform.position = new Vector3(
        //         attacker.transform.position.x,
        //         attacker.transform.position.y,
        //         transform.position.z
        //     );
        // }
        
        public void Shot(Transform attacker, Transform target)
        {
            _target = target;
            _life = 0f;

            transform.position = new Vector3(
                attacker.transform.position.x,
                attacker.transform.position.y,
                transform.position.z
            );
        }

        private void Update()
        {
            if (!_target || !_target.gameObject.activeInHierarchy) {  poolable.DeSpawn(); return; }

            // 타깃 방향으로 이동
            var pos = transform.position;
            var targetPos = _target.position;
            var step = speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(pos, targetPos, step);
            
            // 명중 판정(가까워지면 히트)
            if (Vector3.SqrMagnitude(targetPos - transform.position) <= hitRadius * hitRadius)
            {
                Hit();
                poolable.DeSpawn();
            }

            // 안전 타이머
            _life += Time.deltaTime;
            if (_life > maxLifetime)
            {
                poolable.DeSpawn();
            }
        }

        private void Hit()
        {
            Debug.Log("Projectile.Hit");
            OnHit?.Invoke();
        }
        
        public void OnSpawn()
        {
            
        }

        private void OnDisable()
        {
            OnHit = null;
        }
    }
}