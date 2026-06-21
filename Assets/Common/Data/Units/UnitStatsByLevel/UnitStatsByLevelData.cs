using System;
using System.Collections.Generic;
using Common.Scripts.InspectorHint;
using UnityEngine;
using UnityEngine.Serialization;

namespace Common.Data.Units.UnitStatsByLevel
{
    // ──────────────────────────────────────────────────────────────
    // enum
    // ──────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────
    // UnitStatKind: 능력치 종류의 단일 진실(10종).
    // 정수값을 명시 부여한다 — 스킬 .asset의 StatScaling.statKind가 enum 정수로
    // 직렬화되므로, 생존 멤버는 기존 정수값을 보존하고 병합·제거된 멤버의 정수는
    // 결번으로 둔다(2=구 마법공격력, 4=구 마법방어력, 11=구 상태저항력, 13=구 받는피해감소).
    // ─────────────────────────────────────────────
    public enum UnitStatKind
    {
        [InspectorName("체력")]                  MaxHealth                = 0,
        [InspectorName("공격력")]                Attack                   = 1,
        [InspectorName("방어력")]                Defense                  = 3,
        [InspectorName("공격속도")]              AttackSpeed              = 5,
        [InspectorName("사거리")]                AttackRange              = 6,
        [InspectorName("이동속도")]              MoveSpeed                = 7,
        [InspectorName("치명타 확률")]            CriticalChance           = 8,
        [InspectorName("치명타 피해 배수")]       CriticalDamageMultiplier = 9,
        [InspectorName("스킬 쿨타임 감소")]       CooldownReduction        = 10,
        [InspectorName("입히는 피해 증가")]       DamageDealtIncrease      = 12,
    }

    // ──────────────────────────────────────────────────────────────
    // ScriptableObject
    // ──────────────────────────────────────────────────────────────
    [CreateAssetMenu(fileName = "UnitStatsByLevelData",
                     menuName = "Game/Unit/Unit Stats By Star Level", order = 0)]
    public class UnitStatsByLevelData : ScriptableObject
    {
        [Header("기본 능력치 (별 단계별)")]
        [InspectorHint("체력", InspectorHintPlacement.Right, 100)]
        [SerializeField] private StarStatRecord maxHealth;
        public StarStatRecord MaxHealth => maxHealth;

        // 단일 공격력. 기존 물리공격력 값을 계승한다(FormerlySerializedAs).
        [InspectorHint("공격력", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("physicalAttack")]
        [SerializeField] private StarStatRecord attack;
        public StarStatRecord Attack => attack;

        // 단일 방어력. 기존 물리방어력 값을 계승한다(FormerlySerializedAs).
        [InspectorHint("방어력", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("physicalDefense")]
        [SerializeField] private StarStatRecord defense;
        public StarStatRecord Defense => defense;

        [InspectorHint("공격속도", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("attackSpeedAPS")]
        [SerializeField] private StarStatRecord attackSpeed;
        public StarStatRecord AttackSpeed => attackSpeed;

        [InspectorHint("사거리", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("attackRangeTiles")]
        [SerializeField] private StarStatRecord attackRange;
        public StarStatRecord AttackRange => attackRange;

        [InspectorHint("이동속도", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("moveSpeedTilesPerSec")]
        [SerializeField] private StarStatRecord moveSpeed;
        public StarStatRecord MoveSpeed => moveSpeed;

        [Header("치명/쿨감/피해증가 (비율은 0~1 권장)")]
        [InspectorHint("치명타 확률(%)", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("criticalChance")]
        [SerializeField] private StarStatRecord criticalChance;
        public StarStatRecord CriticalChance => criticalChance;

        [InspectorHint("치명타 피해 배수", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("criticalDamageMultiplier")]
        [SerializeField] private StarStatRecord criticalDamageMultiplier;
        public StarStatRecord CriticalDamageMultiplier => criticalDamageMultiplier;

        [InspectorHint("스킬 쿨타임 감소", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("cooldownReduction")]
        [SerializeField] private StarStatRecord cooldownReduction;
        public StarStatRecord CooldownReduction => cooldownReduction;

        [InspectorHint("입히는 피해 증가", InspectorHintPlacement.Right, 100)]
        [FormerlySerializedAs("damageDealtIncrease")]
        [SerializeField] private StarStatRecord damageDealtIncrease;
        public StarStatRecord DamageDealtIncrease => damageDealtIncrease;

        // enum으로 조회 (필요 시)
        public float GetStat(UnitStatKind kind, int star) => kind switch
        {
            UnitStatKind.MaxHealth                => maxHealth.GetValue(star),
            UnitStatKind.Attack                   => attack.GetValue(star),
            UnitStatKind.Defense                  => defense.GetValue(star),
            UnitStatKind.AttackSpeed              => attackSpeed.GetValue(star),
            UnitStatKind.AttackRange              => attackRange.GetValue(star),
            UnitStatKind.MoveSpeed                => moveSpeed.GetValue(star),
            UnitStatKind.CriticalChance           => criticalChance.GetValue(star),
            UnitStatKind.CriticalDamageMultiplier => criticalDamageMultiplier.GetValue(star),
            UnitStatKind.CooldownReduction        => cooldownReduction.GetValue(star),
            UnitStatKind.DamageDealtIncrease      => damageDealtIncrease.GetValue(star),
            _ => 0f
        };

        /// <summary>해당 능력치의 성급 기준값이 자산에 입력돼 있는지 반환한다.</summary>
        public bool HasStat(UnitStatKind kind) => kind switch
        {
            UnitStatKind.MaxHealth                => maxHealth.HasAnyValue,
            UnitStatKind.Attack                   => attack.HasAnyValue,
            UnitStatKind.Defense                  => defense.HasAnyValue,
            UnitStatKind.AttackSpeed              => attackSpeed.HasAnyValue,
            UnitStatKind.AttackRange              => attackRange.HasAnyValue,
            UnitStatKind.MoveSpeed                => moveSpeed.HasAnyValue,
            UnitStatKind.CriticalChance           => criticalChance.HasAnyValue,
            UnitStatKind.CriticalDamageMultiplier => criticalDamageMultiplier.HasAnyValue,
            UnitStatKind.CooldownReduction        => cooldownReduction.HasAnyValue,
            UnitStatKind.DamageDealtIncrease      => damageDealtIncrease.HasAnyValue,
            _ => false
        };
    }

    // ──────────────────────────────────────────────────────────────
    // StarStatRecord: 내부 필드에도 항상 보이는 힌트 추가
    // ──────────────────────────────────────────────────────────────
    [Serializable]
    public struct StarStatRecord
    {
        [Tooltip("1성부터 순서대로 입력하는 값 리스트 (예: 1~3성 값)")]
        [SerializeField] private List<float> baseValuesByStar;

        [Tooltip("기록된 마지막 별 이후(예: 3성 초과) 매 성마다 더해질 고정 증가치")]
        [SerializeField] private float additionalPerExtraStar;

        public IReadOnlyList<float> BaseValuesByStar => baseValuesByStar;
        public float AdditionalPerExtraStar => additionalPerExtraStar;

        public bool HasAnyValue => baseValuesByStar != null && baseValuesByStar.Count > 0;

        /// <summary>
        /// star(1부터)에 해당하는 값을 반환.
        /// - 리스트에 값이 없으면 0
        /// - star가 리스트 길이를 초과하면: 마지막값 + (초과별 * 추가증가치)
        /// </summary>
        public float GetValue(int star)
        {
            if (!HasAnyValue) return 0f;
            if (star < 1) star = 1;

            int count = baseValuesByStar.Count;
            if (star <= count) return baseValuesByStar[star - 1];

            int extra = star - count;
            return baseValuesByStar[count - 1] + additionalPerExtraStar * extra;
        }
    }
}


