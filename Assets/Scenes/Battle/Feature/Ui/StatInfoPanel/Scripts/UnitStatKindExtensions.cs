using Common.Data.Units.UnitStatsByLevel;
using Scenes.Battle.Feature.Units.UnitStats;

namespace Scenes.Battle.Feature.Ui.StatInfoPanel
{
    public static class UnitStatKindExtensions
    {
        public static string GetDisplayName(this UnitStatKind kind) => kind switch
        {
            UnitStatKind.MaxHealth => "체력",
            UnitStatKind.Attack => "공격력",
            UnitStatKind.Defense => "방어력",
            UnitStatKind.AttackSpeed => "공격속도",
            UnitStatKind.AttackRange => "사거리",
            UnitStatKind.MoveSpeed => "이동속도",
            UnitStatKind.CriticalChance => "치명타 확률",
            UnitStatKind.CriticalDamageMultiplier => "치명타 피해 배수",
            UnitStatKind.CooldownReduction => "스킬 쿨타임 감소",
            UnitStatKind.DamageDealtIncrease => "입히는 피해 증가",
            _ => kind.ToString()
        };

        /// <summary>능력치 단위 정의에 맞는 표기 문자열을 반환한다. 값 강제와 같은 단위 정의를 참조한다.</summary>
        public static string FormatStatValue(this StatUnit unit, float value)
        {
            return unit switch
            {
                StatUnit.Percent => $"{value * 100f:F0}%",
                StatUnit.Multiplier => $"{value:F1}x",
                StatUnit.Float => $"{value:F2}",
                _ => $"{value:F0}"   // Integer
            };
        }
    }
}
