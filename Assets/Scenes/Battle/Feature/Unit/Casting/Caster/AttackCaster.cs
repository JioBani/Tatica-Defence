using System;
using Common.Data.Units.UnitStatsByLevel;
using Common.Scripts.DynamicRepeater;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Unit.Attackers.AttackContexts;
using Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos;
using Scenes.Battle.Feature.Unit.Skills.Castables;
using Scenes.Battle.Feature.Units.ActionStates;
using Scenes.Battle.Feature.Units.Attackables;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Skills.Caster
{
    public class AttackCaster : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private float attackSpeed;
        [SerializeField] private Units.Unit unit;
        [SerializeField] private ActionStateController actionStateController;
        public Units.Unit Unit => unit;
        
        private CircleCollider2D _circleCollider2D;
        private Victim _victim;
        public Victim Victim => _victim;

        public Action<Victim> OnTargetEnter;
        public Action<Victim> OnTargetExit;

        private DynamicRepeater _attackRepeater;
        private AttackContextDto _attackContextDto;

        private AttackCast _attackCast;

        private void Awake()
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();
            unit.OnSpawnEvent += SetStats;

            var actionEvent = actionStateController
                .GetStateBase(ActionStateType.Attack)
                .Event;
            
            actionEvent.Add(StateBaseEventType.Enter, (_,_) => StartRepeat());
            actionEvent.Add(StateBaseEventType.Exit, (_,_) => EndAttackRepeat());

            //_attackCast = new AttackCast(this);
        }

        private void Update()
        {
            if (_victim && _victim.Unit.ActionStateController.CurrentState.StateType == ActionStateType.Downed)
            {
                ReleaseVictim();
            }
        }

        //TODO: 동적 스탯 변경을 적용하기
        private void SetStats(Units.Unit unit)
        {
            _circleCollider2D.radius = unit.UnitLoadOutData.Stats.GetStat(UnitStatKind.AttackRange, 0);
            attackSpeed = unit.UnitLoadOutData.Stats.GetStat(UnitStatKind.AttackSpeed, 0);
            
            _attackRepeater?.Dispose();
            _attackRepeater = new DynamicRepeater(
                intervalNow: () => TimeSpan.FromSeconds(1 / attackSpeed), 
                job : async () => Attack()
            );
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_victim == null && other.CompareTag("Victim"))
            {
                Victim newVictim = other.GetComponent<Victim>();

                if (
                    newVictim.Unit.fraction != Unit.fraction && 
                    newVictim.Unit.ActionStateController.CurrentState.StateType != ActionStateType.Downed
                )
                {
                    _victim = newVictim;
                    OnTargetEnter?.Invoke(_victim);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (
                _victim != null && 
                other.CompareTag("Victim") && 
                _victim == other.GetComponent<Victim>()
            )
            {
                ReleaseVictim();
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
                _attackCast.Cast();
                // AttackContextDto attackContextDto = new AttackContextDto(
                //     damage: unit.StatSheet.PhysicalAttack.CurrentValue,
                //     attacker: this,
                //     victim: _victim
                // );
                //
                // var context = AttackContextFactory.Instance.GenerateRanged(attackContextDto);
                //
                // context.TryAttack();
            }
        }

        private void ReleaseVictim()
        {
            Victim exitVictim = _victim;
            _victim = null;
            OnTargetExit?.Invoke(exitVictim);
        }
    }
}