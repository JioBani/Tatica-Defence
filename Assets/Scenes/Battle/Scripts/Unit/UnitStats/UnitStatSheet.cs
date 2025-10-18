using System.Collections.Generic;
using Common.Data.Units.UnitStatsByLevel;

// 별 레벨에 따라 다시 계산 필요한 경우 설계가 변경되는지 확인 필요
namespace Scenes.Battle.Scripts.Unit.UnitStats
{
    /// <summary>
    /// 유닛의 런타임 스탯 묶음(옵저버블).
    /// - 생성자에서 UnitStatsByLevelData와 star(기본 1)를 받아 초기화
    /// - 별 변화 시 ApplyStar(star)로 재적용 가능
    /// - enum으로 스탯 접근: Get(UnitStatKind)
    /// </summary>
    public class UnitStatSheet
    {
        // 원본 데이터(별 단계별 기본값)
        private readonly UnitStatsByLevelData _data;

        // 개별 스탯(옵저버블)
        public UnitStats<float> MaxHealth;
        public UnitStats<float> PhysicalAttack;
        public UnitStats<float> MagicAttack;
        public UnitStats<float> PhysicalDefense;
        public UnitStats<float> MagicDefense;
        public UnitStats<float> AttackSpeed;
        public UnitStats<float> AttackRange;
        public UnitStats<float> MoveSpeed;

        public UnitStats<float> CriticalChance;
        public UnitStats<float> CriticalDamageMultiplier;
        public UnitStats<float> CooldownReduction;
        public UnitStats<float> StatusResistance;
        public UnitStats<float> DamageDealtIncrease;

        // 생성자: 기본 1성으로 초기화
        public UnitStatSheet(UnitStatsByLevelData data, int star = 1)
        {
            _data = data;
            ApplyStar(star);
        }

        /// <summary>
        /// 별(스타) 단계에 맞춰 모든 스탯을 재구성.
        /// UnitStats<T>.OriginalValue가 불변이므로, 인스턴스를 새로 만들어 교체한다.
        /// </summary>
        public void ApplyStar(int star)
        {
            MaxHealth               = new UnitStats<float>(_data.MaxHealth.GetValue(star));
            PhysicalAttack          = new UnitStats<float>(_data.PhysicalAttack.GetValue(star));
            MagicAttack             = new UnitStats<float>(_data.MagicAttack.GetValue(star));
            PhysicalDefense         = new UnitStats<float>(_data.PhysicalDefense.GetValue(star));
            MagicDefense            = new UnitStats<float>(_data.MagicDefense.GetValue(star));
            AttackSpeed             = new UnitStats<float>(_data.AttackSpeed.GetValue(star));
            AttackRange             = new UnitStats<float>(_data.AttackRange.GetValue(star));
            MoveSpeed               = new UnitStats<float>(_data.MoveSpeed.GetValue(star));

            CriticalChance          = new UnitStats<float>(_data.CriticalChance.GetValue(star));
            CriticalDamageMultiplier= new UnitStats<float>(_data.CriticalDamageMultiplier.GetValue(star));
            CooldownReduction       = new UnitStats<float>(_data.CooldownReduction.GetValue(star));
            StatusResistance        = new UnitStats<float>(_data.StatusResistance.GetValue(star));
            DamageDealtIncrease     = new UnitStats<float>(_data.DamageDealtIncrease.GetValue(star));
        }

        /// <summary>
        /// enum으로 해당 스탯(UnitStats<float>)을 얻는다.
        /// </summary>
        public UnitStats<float> Get(UnitStatKind kind) => kind switch
        {
            UnitStatKind.MaxHealth                => MaxHealth,
            UnitStatKind.PhysicalAttack           => PhysicalAttack,
            UnitStatKind.MagicAttack              => MagicAttack,
            UnitStatKind.PhysicalDefense          => PhysicalDefense,
            UnitStatKind.MagicDefense             => MagicDefense,
            UnitStatKind.AttackSpeed              => AttackSpeed,
            UnitStatKind.AttackRange              => AttackRange,
            UnitStatKind.MoveSpeed                => MoveSpeed,
            UnitStatKind.CriticalChance           => CriticalChance,
            UnitStatKind.CriticalDamageMultiplier => CriticalDamageMultiplier,
            UnitStatKind.CooldownReduction        => CooldownReduction,
            UnitStatKind.StatusResistance         => StatusResistance,
            UnitStatKind.DamageDealtIncrease      => DamageDealtIncrease,
            _ => null
        };

        /// <summary>
        /// 모든 스탯을 열거(편의).
        /// </summary>
        public IEnumerable<(UnitStatKind kind, UnitStats<float> stat)> Enumerate()
        {
            yield return (UnitStatKind.MaxHealth,                MaxHealth);
            yield return (UnitStatKind.PhysicalAttack,           PhysicalAttack);
            yield return (UnitStatKind.MagicAttack,              MagicAttack);
            yield return (UnitStatKind.PhysicalDefense,          PhysicalDefense);
            yield return (UnitStatKind.MagicDefense,             MagicDefense);
            yield return (UnitStatKind.AttackSpeed,              AttackSpeed);
            yield return (UnitStatKind.AttackRange,              AttackRange);
            yield return (UnitStatKind.MoveSpeed,                MoveSpeed);
            yield return (UnitStatKind.CriticalChance,           CriticalChance);
            yield return (UnitStatKind.CriticalDamageMultiplier, CriticalDamageMultiplier);
            yield return (UnitStatKind.CooldownReduction,        CooldownReduction);
            yield return (UnitStatKind.StatusResistance,         StatusResistance);
            yield return (UnitStatKind.DamageDealtIncrease,      DamageDealtIncrease);
        }
    }
}
