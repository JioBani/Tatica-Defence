using NUnit.Framework;
using Scenes.Battle.Feature.Units.Damage;
using Scenes.Battle.Feature.Units.UnitStats.UnitStatSheets;

namespace Tests.Editor
{
    public class DamageCalculatorTests
    {
        private UnitStatSheet _attacker;
        private UnitStatSheet _victim;

        [SetUp]
        public void SetUp()
        {
            _attacker = new UnitStatSheet();
            _attacker.Attack.SetBaseValue(100f);
            _attacker.CriticalChance.SetBaseValue(0f);
            _attacker.CriticalDamageMultiplier.SetBaseValue(1.5f);
            _attacker.DamageDealtIncrease.SetBaseValue(0f);

            _victim = new UnitStatSheet();
            _victim.Defense.SetBaseValue(0f);
        }

        [Test]
        public void BaseDamage_NoCrit_NoDef()
        {
            // 100 * 1.0 = 100
            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: false);
            Assert.AreEqual(100f, result);
        }

        [Test]
        public void SkillCoefficient_MultipliesBase()
        {
            // 100 * 1.5 = 150
            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: false, skillCoefficient: 1.5f);
            Assert.AreEqual(150f, result);
        }

        [Test]
        public void CriticalHit_MultipliesByCritDamage()
        {
            // 100 * 1.5(치명타) = 150
            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: true);
            Assert.AreEqual(150f, result);
        }

        [Test]
        public void Defense_ReducesDamage()
        {
            // 100 * (1 - 0.3) = 70
            _victim.Defense.SetBaseValue(0.3f);
            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: false);
            Assert.AreEqual(70f, result);
        }

        [Test]
        public void DamageIncrease_MultipliesDamage()
        {
            // 100 * (1 + 0.2) = 120
            _attacker.DamageDealtIncrease.SetBaseValue(0.2f);
            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: false);
            Assert.AreEqual(120f, result);
        }

        [Test]
        public void FullCombo_AllFactors()
        {
            // 받는 피해 감소 단계 제거 후 파이프라인:
            // 100 * 1.5(crit) * (1-0.3)(def) * (1+0.2)(inc)
            // = 100 * 1.5 * 0.7 * 1.2 = 126 → floor = 126
            _victim.Defense.SetBaseValue(0.3f);
            _attacker.DamageDealtIncrease.SetBaseValue(0.2f);

            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: true);
            Assert.AreEqual(126f, result);
        }

        [Test]
        public void Floor_TruncatesDecimal()
        {
            // 100 * (1 - 0.33) = 67.0 → 67 (정수 결과도 검증)
            _victim.Defense.SetBaseValue(0.33f);
            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: false);
            Assert.AreEqual(67f, result);
        }

        [Test]
        public void MinimumZero_NeverNegative()
        {
            // 방어력이 100% 이상이면 최소 0
            _victim.Defense.SetBaseValue(1.5f);
            float result = DamageCalculator.Calculate(_attacker, _victim, isCritical: false);
            Assert.AreEqual(0f, result);
        }

        // ── baseDamage overload 테스트 ──

        [Test]
        public void BaseDamageOverload_NoCrit_NoDef()
        {
            // 200 그대로 반환
            float result = DamageCalculator.Calculate(200f, _attacker, _victim, isCritical: false);
            Assert.AreEqual(200f, result);
        }

        [Test]
        public void BaseDamageOverload_WithCrit()
        {
            // 200 * 1.5(치명타) = 300
            float result = DamageCalculator.Calculate(200f, _attacker, _victim, isCritical: true);
            Assert.AreEqual(300f, result);
        }

        [Test]
        public void BaseDamageOverload_WithDefense()
        {
            // 200 * (1 - 0.3) = 140
            _victim.Defense.SetBaseValue(0.3f);
            float result = DamageCalculator.Calculate(200f, _attacker, _victim, isCritical: false);
            Assert.AreEqual(140f, result);
        }

        [Test]
        public void BaseDamageOverload_FullPipeline()
        {
            // 받는 피해 감소 단계 제거 후:
            // 200 * 1.5(crit) * (1-0.3)(def) * (1+0.2)(inc)
            // = 200 * 1.5 * 0.7 * 1.2 = 252 → floor = 252
            _victim.Defense.SetBaseValue(0.3f);
            _attacker.DamageDealtIncrease.SetBaseValue(0.2f);

            float result = DamageCalculator.Calculate(200f, _attacker, _victim, isCritical: true);
            Assert.AreEqual(252f, result);
        }

        [Test]
        public void BaseDamageOverload_MinimumZero()
        {
            // 방어력 100% 이상 → 최소 0
            _victim.Defense.SetBaseValue(1.5f);
            float result = DamageCalculator.Calculate(200f, _attacker, _victim, isCritical: false);
            Assert.AreEqual(0f, result);
        }
    }
}
