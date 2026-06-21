using Common.Data.Units.UnitStatsByLevel;
using NUnit.Framework;
using Scenes.Battle.Feature.Ui.StatInfoPanel;

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

        // ── FormatStatValue: 퍼센트 표시 스탯 ──

        [Test]
        public void FormatStatValue_CriticalChance_FormatsAsPercent()
        {
            Assert.AreEqual("25%", UnitStatKind.CriticalChance.FormatStatValue(0.25f));
        }

        [Test]
        public void FormatStatValue_CooldownReduction_FormatsAsPercent()
        {
            Assert.AreEqual("10%", UnitStatKind.CooldownReduction.FormatStatValue(0.1f));
        }

        [Test]
        public void FormatStatValue_DamageDealtIncrease_FormatsAsPercent()
        {
            Assert.AreEqual("15%", UnitStatKind.DamageDealtIncrease.FormatStatValue(0.15f));
        }

        [Test]
        public void FormatStatValue_PercentZero_Shows0Percent()
        {
            Assert.AreEqual("0%", UnitStatKind.CriticalChance.FormatStatValue(0f));
        }

        [Test]
        public void FormatStatValue_PercentFull_Shows100Percent()
        {
            Assert.AreEqual("100%", UnitStatKind.CriticalChance.FormatStatValue(1f));
        }

        // ── FormatStatValue: 소수점 2자리 스탯 (공격속도) ──

        [Test]
        public void FormatStatValue_AttackSpeed_FormatsTwoDecimals()
        {
            Assert.AreEqual("1.50", UnitStatKind.AttackSpeed.FormatStatValue(1.5f));
        }

        [Test]
        public void FormatStatValue_AttackSpeed_SmallValue()
        {
            Assert.AreEqual("0.80", UnitStatKind.AttackSpeed.FormatStatValue(0.8f));
        }

        // ── FormatStatValue: 배수 표시 스탯 (치명타 피해 배수) ──

        [Test]
        public void FormatStatValue_CriticalDamageMultiplier_FormatsAsMultiplier()
        {
            Assert.AreEqual("1.5x", UnitStatKind.CriticalDamageMultiplier.FormatStatValue(1.5f));
        }

        [Test]
        public void FormatStatValue_CriticalDamageMultiplier_DefaultValue()
        {
            Assert.AreEqual("1.0x", UnitStatKind.CriticalDamageMultiplier.FormatStatValue(1f));
        }

        // ── FormatStatValue: 정수 표시 스탯 ──

        [Test]
        public void FormatStatValue_MaxHealth_FormatsAsInteger()
        {
            Assert.AreEqual("1000", UnitStatKind.MaxHealth.FormatStatValue(1000f));
        }

        [Test]
        public void FormatStatValue_Attack_FormatsAsInteger()
        {
            Assert.AreEqual("150", UnitStatKind.Attack.FormatStatValue(150f));
        }

        [Test]
        public void FormatStatValue_MoveSpeed_FormatsAsInteger()
        {
            Assert.AreEqual("3", UnitStatKind.MoveSpeed.FormatStatValue(3.2f));
        }

        [Test]
        public void FormatStatValue_AttackRange_FormatsAsInteger()
        {
            Assert.AreEqual("5", UnitStatKind.AttackRange.FormatStatValue(5f));
        }
    }
}
