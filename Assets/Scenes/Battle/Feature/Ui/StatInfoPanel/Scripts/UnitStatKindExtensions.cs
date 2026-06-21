using Common.Data.Units.UnitStatsByLevel;

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

        public static string FormatStatValue(this UnitStatKind kind, float value)
        {
            return kind switch
            {
                UnitStatKind.CriticalChance or
                UnitStatKind.CooldownReduction or
                UnitStatKind.DamageDealtIncrease => $"{value * 100f:F0}%",
                UnitStatKind.AttackSpeed => $"{value:F2}",
                UnitStatKind.CriticalDamageMultiplier => $"{value:F1}x",
                _ => $"{value:F0}"
            };
        }
    }
}
