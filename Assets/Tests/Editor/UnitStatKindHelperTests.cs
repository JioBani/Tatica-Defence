using Common.Data.Units.UnitStatsByLevel;
using NUnit.Framework;
using Scenes.Battle.Feature.Ui.StatInfoPanel;
using Scenes.Battle.Feature.Units.UnitStats;

namespace Tests.Editor
{
    public class UnitStatKindHelperTests
    {
        // ── GetDisplayName (10종 단일화 기준) ──

        [Test]
        public void GetDisplayName_MaxHealth_Returns체력()
        {
            Assert.AreEqual("체력", UnitStatKind.MaxHealth.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_Attack_Returns공격력()
        {
            Assert.AreEqual("공격력", UnitStatKind.Attack.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_Defense_Returns방어력()
        {
            Assert.AreEqual("방어력", UnitStatKind.Defense.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_AttackSpeed_Returns공격속도()
        {
            Assert.AreEqual("공격속도", UnitStatKind.AttackSpeed.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_AttackRange_Returns사거리()
        {
            Assert.AreEqual("사거리", UnitStatKind.AttackRange.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_MoveSpeed_Returns이동속도()
        {
            Assert.AreEqual("이동속도", UnitStatKind.MoveSpeed.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_CriticalChance_Returns치명타확률()
        {
            Assert.AreEqual("치명타 확률", UnitStatKind.CriticalChance.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_CriticalDamageMultiplier_Returns치명타피해배수()
        {
            Assert.AreEqual("치명타 피해 배수", UnitStatKind.CriticalDamageMultiplier.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_CooldownReduction_Returns스킬쿨타임감소()
        {
            Assert.AreEqual("스킬 쿨타임 감소", UnitStatKind.CooldownReduction.GetDisplayName());
        }

        [Test]
        public void GetDisplayName_DamageDealtIncrease_Returns입히는피해증가()
        {
            Assert.AreEqual("입히는 피해 증가", UnitStatKind.DamageDealtIncrease.GetDisplayName());
        }

        // ── FormatStatValue: 단위(StatUnit)별 표기 ──
        // FormatStatValue 는 stat-unit-range(Task-3)에서 단위(StatUnit) 기반 시그니처로 변경됐다.
        // 능력치 종류→단위 매핑·표시 정합은 StatUnitRangeTests(DoD-S10)가 별도 검증한다.

        [Test]
        public void FormatStatValue_Percent_FormatsWithPercentSign()
        {
            Assert.AreEqual("25%", StatUnit.Percent.FormatStatValue(0.25f));
        }

        [Test]
        public void FormatStatValue_PercentZero_Shows0Percent()
        {
            Assert.AreEqual("0%", StatUnit.Percent.FormatStatValue(0f));
        }

        [Test]
        public void FormatStatValue_PercentFull_Shows100Percent()
        {
            Assert.AreEqual("100%", StatUnit.Percent.FormatStatValue(1f));
        }

        [Test]
        public void FormatStatValue_Float_FormatsTwoDecimals()
        {
            Assert.AreEqual("1.50", StatUnit.Float.FormatStatValue(1.5f));
        }

        [Test]
        public void FormatStatValue_Float_SmallValue()
        {
            Assert.AreEqual("0.80", StatUnit.Float.FormatStatValue(0.8f));
        }

        [Test]
        public void FormatStatValue_Multiplier_FormatsWithX()
        {
            Assert.AreEqual("1.5x", StatUnit.Multiplier.FormatStatValue(1.5f));
        }

        [Test]
        public void FormatStatValue_Multiplier_DefaultValue()
        {
            Assert.AreEqual("1.0x", StatUnit.Multiplier.FormatStatValue(1f));
        }

        [Test]
        public void FormatStatValue_Integer_FormatsAsInteger()
        {
            Assert.AreEqual("1000", StatUnit.Integer.FormatStatValue(1000f));
        }
    }
}
