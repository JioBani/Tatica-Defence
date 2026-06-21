// ─────────────────────────────────────────────
// UnitStatMetadataCatalog: 능력치 종류별 단위·범위·기본값·합산 방식의 단일 진실.
// 값 강제(UnitStat)와 표시(UI)가 모두 이 정의를 참조한다.
// 능력치 범위/단위가 바뀌면 이 Get 의 항목만 고친다.
// ─────────────────────────────────────────────
using System;
using Common.Data.Units.UnitStatsByLevel;

namespace Scenes.Battle.Feature.Units.UnitStats
{
    /// <summary>능력치 종류별 메타데이터(단위·범위·기본값·합산 방식)의 단일 진실 카탈로그.</summary>
    public class UnitStatMetadataCatalog
    {
        /// <summary>능력치 종류에 해당하는 메타데이터를 반환한다.</summary>
        public UnitStatMetadata Get(UnitStatKind kind)
        {
            // 비율(Percent)은 0~1로 저장한다(표시 시 ×100%). 상한 무제한은 PositiveInfinity.
            return kind switch
            {
                UnitStatKind.MaxHealth                => new UnitStatMetadata(StatUnit.Integer, 1f, float.PositiveInfinity, 1f),
                UnitStatKind.Attack                   => new UnitStatMetadata(StatUnit.Integer, 0f, float.PositiveInfinity, 0f),
                UnitStatKind.Defense                  => new UnitStatMetadata(StatUnit.Percent, 0f, 0.95f, 0f, StatCalculationMode.SeparatedMultiplicative),
                UnitStatKind.AttackSpeed              => new UnitStatMetadata(StatUnit.Float, 0.1f, float.PositiveInfinity, 0.1f),
                UnitStatKind.AttackRange              => new UnitStatMetadata(StatUnit.Float, 0f, float.PositiveInfinity, 0f),
                UnitStatKind.MoveSpeed                => new UnitStatMetadata(StatUnit.Float, 0f, float.PositiveInfinity, 0f),
                UnitStatKind.CriticalChance           => new UnitStatMetadata(StatUnit.Percent, 0f, 1f, 0f),
                UnitStatKind.CriticalDamageMultiplier => new UnitStatMetadata(StatUnit.Multiplier, 1f, float.PositiveInfinity, 1.5f),
                UnitStatKind.CooldownReduction        => new UnitStatMetadata(StatUnit.Percent, 0f, 0.9f, 0f),
                UnitStatKind.DamageDealtIncrease      => new UnitStatMetadata(StatUnit.Percent, 0f, float.PositiveInfinity, 0f),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind,
                        "[UnitStatMetadataCatalog] 정의되지 않은 UnitStatKind 입니다. 10종 메타데이터를 모두 정의해야 합니다.")
            };
        }
    }
}
