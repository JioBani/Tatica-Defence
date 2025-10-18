using System;
using Common.Data.Units.UnitStatsByLevel;
using Common.Scripts.DynamicRepeater;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Rounds.Unit.ActionState;
using Scenes.Battle.Feature.Units.Attackable;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.Attackers
{
    public class Attacker : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private float attackSpeed;
        [SerializeField] private Unit unit;
        [SerializeField] private ActionStateController actionStateController;
        public Unit Unit => unit;
        
        private CircleCollider2D _circleCollider2D;
        private Victim _victim;

        public Action<Victim> OnTargetEnter;
        public Action<Victim> OnTargetExit;

        private DynamicRepeater _attackRepeater;

        private void Awake()
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();
            unit.OnSpawnEvent += SetStats;

            var actionEvent = actionStateController
                .GetStateBase(ActionStateType.Attack)
                .Event;
            
            actionEvent.Add(StateBaseEventType.Enter, (_,_) => StartRepeat());
            actionEvent.Add(StateBaseEventType.Exit, (_,_) => EndAttackRepeat());
        }

        //TODO: 동적 스탯 변경을 적용하기
        private void SetStats(Unit unit)
        {
            _circleCollider2D.radius = unit.UnitLoadOutData.Stats.GetStat(UnitStatKind.AttackRange, 0);
            attackSpeed = unit.UnitLoadOutData.Stats.GetStat(UnitStatKind.AttackSpeed, 0);
            
            _attackRepeater?.Dispose();
            _attackRepeater = new DynamicRepeater(
                intervalNow: () => TimeSpan.FromSeconds(attackSpeed), 
                job : async () => Attack()
            );
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_victim == null && other.CompareTag("Victim"))
            {
                Victim newVictim = other.GetComponent<Victim>();

                if (newVictim.Unit.fraction != Unit.fraction)
                {
                    _victim = newVictim;
                    
                    OnTargetEnter?.Invoke(_victim);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_victim != null && other.CompareTag("Victim") && _victim == other.GetComponent<Victim>())
            {
                Victim exitVictim = _victim;
                _victim = null;
                OnTargetExit.Invoke(exitVictim);
            }
        }

        private void OnDestroy()
        {
            unit.OnSpawnEvent -= SetStats;
            _attackRepeater.Dispose();
        }

        private void StartRepeat()
        {
            _attackRepeater.Start();
        }

        private void EndAttackRepeat()
        {
            _attackRepeater.Stop();
        }

        private void Attack()
        {
            if (_victim != null)
            {
                _victim.Hit(new AttackContext(10));
            }
            Debug.Log("공격");
        }
    }
}
