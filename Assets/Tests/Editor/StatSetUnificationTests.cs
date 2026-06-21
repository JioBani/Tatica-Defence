using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Data.Units.UnitStatsByLevel;
using NUnit.Framework;
using Scenes.Battle.Feature.Ui.StatInfoPanel;
using Scenes.Battle.Feature.Units.Damage;
using Scenes.Battle.Feature.Units.UnitStats.UnitStatSheets;

namespace Tests.Editor
{
    /// <summary>
    /// 구현 단위 stat-set-unification (Task-2, SR 그룹 A / DoD-S1~S7) 의 요구사항 테스트.
    /// 능력치 종류 집합 10종 단일화 + 단일 공격력·방어력 + 단일 채널 데미지 파이프라인을 검증한다.
    /// 검증 기준은 UR Req-2·4 / SR DoD-S1~S7 의 완료 정의이며, 구현 코드 동작이 아니다.
    /// </summary>
    public class StatSetUnificationTests
    {
        // SR DoD-S1 이 명시한 정확히 10종의 능력치 종류명.
        private static readonly string[] Expected10KindNames =
        {
            nameof(UnitStatKind.MaxHealth),
            nameof(UnitStatKind.Attack),
            nameof(UnitStatKind.Defense),
            nameof(UnitStatKind.AttackSpeed),
            nameof(UnitStatKind.AttackRange),
            nameof(UnitStatKind.MoveSpeed),
            nameof(UnitStatKind.CriticalChance),
            nameof(UnitStatKind.CriticalDamageMultiplier),
            nameof(UnitStatKind.CooldownReduction),
            nameof(UnitStatKind.DamageDealtIncrease),
        };

        // ── DoD-S1: 능력치 종류 단일 진실이 정확히 10종만 열거 ──

        /// <summary>DoD-S1: 능력치 종류 단일 진실(enum)이 정확히 10종만 가진다.</summary>
        [Test]
        public void Test_S1_EnumHasExactly10Kinds()
        {
            int kindCount = Enum.GetValues(typeof(UnitStatKind)).Length;

            Assert.That(kindCount, Is.EqualTo(10));
        }

        /// <summary>DoD-S1: enum 의 종류 집합이 명세된 10종과 정확히 일치한다.</summary>
        [Test]
        public void Test_S1_EnumContainsExactlyThe10SpecifiedKinds()
        {
            var actualNames = Enum.GetNames(typeof(UnitStatKind));

            Assert.That(actualNames, Is.EquivalentTo(Expected10KindNames));
        }

        // ── DoD-S2: 단일 공격력·단일 방어력 (물리/마법 분리 없음) ──

        /// <summary>DoD-S2: 공격력·방어력이 물리/마법 분리 없이 단일 종류로만 존재한다.</summary>
        [Test]
        [TestCase("PhysicalAttack")]
        [TestCase("MagicAttack")]
        [TestCase("PhysicalDefense")]
        [TestCase("MagicDefense")]
        public void Test_S2_SplitAttackDefenseKindsAbsent(string splitKindName)
        {
            var actualNames = Enum.GetNames(typeof(UnitStatKind));

            Assert.That(actualNames, Does.Not.Contain(splitKindName));
        }

        /// <summary>DoD-S2: 단일 공격력(Attack)·단일 방어력(Defense)이 종류 집합에 존재한다.</summary>
        [Test]
        [TestCase("Attack")]
        [TestCase("Defense")]
        public void Test_S2_SingleAttackAndDefenseKindsPresent(string singleKindName)
        {
            var actualNames = Enum.GetNames(typeof(UnitStatKind));

            Assert.That(actualNames, Does.Contain(singleKindName));
        }

        // ── DoD-S1/S7: 상태저항력·받는 피해 감소 제거 ──

        /// <summary>DoD-S1·S7: 상태저항력·받는 피해 감소 종류가 집합에서 제거되었다.</summary>
        [Test]
        [TestCase("StatusResistance")]
        [TestCase("DamageReduction")]
        public void Test_S1_RemovedKindsAbsentFromEnum(string removedKindName)
        {
            var actualNames = Enum.GetNames(typeof(UnitStatKind));

            Assert.That(actualNames, Does.Not.Contain(removedKindName));
        }

        // ── DoD-S4: 유닛 스탯 시트의 보유·조회·순회 집합이 10종과 일치 ──

        /// <summary>DoD-S4: 스탯 시트의 순회(Enumerate)가 10종 능력치 집합과 정확히 일치한다.</summary>
        [Test]
        public void Test_S4_SheetEnumeratesExactlyThe10Kinds()
        {
            var sheet = new UnitStatSheet();

            var enumeratedKinds = sheet.Enumerate().Select(entry => entry.kind).ToArray();

            Assert.That(enumeratedKinds, Is.EquivalentTo(Enum.GetValues(typeof(UnitStatKind)).Cast<UnitStatKind>()));
        }

        /// <summary>DoD-S4: 스탯 시트가 10종 각각에 대해 조회(Get) 시 능력치를 반환한다.</summary>
        [Test]
        public void Test_S4_GetReturnsStatForEvery10Kinds()
        {
            var sheet = new UnitStatSheet();

            foreach (UnitStatKind kind in Enum.GetValues(typeof(UnitStatKind)))
            {
                Assert.That(sheet.Get(kind), Is.Not.Null, $"{kind} 조회 결과가 null");
            }
        }

        // ── DoD-S3: 데미지 계산은 단일 공격력·단일 방어력 채널만 사용 ──

        /// <summary>DoD-S3: 데미지가 단일 공격력을 기본 데미지의 출발점으로 사용한다(데미지 타입 분기 없음).</summary>
        [Test]
        public void Test_S3_DamageUsesSingleAttackChannel()
        {
            const float attackPower = 100f;
            var attacker = MakeAttacker(attack: attackPower);
            var victim = MakeVictim(defense: 0f);

            float result = DamageCalculator.Calculate(attacker, victim, isCritical: false);

            Assert.That(result, Is.EqualTo(attackPower));
        }

        /// <summary>DoD-S3: 데미지가 단일 방어력 채널로 경감된다.</summary>
        [Test]
        public void Test_S3_DamageReducedBySingleDefenseChannel()
        {
            const float attackPower = 100f;
            const float defenseRatio = 0.3f;
            var attacker = MakeAttacker(attack: attackPower);
            var victim = MakeVictim(defense: defenseRatio);

            float result = DamageCalculator.Calculate(attacker, victim, isCritical: false);

            // 100 * (1 - 0.3) = 70
            Assert.That(result, Is.EqualTo(70f));
        }

        /// <summary>DoD-S3: 평타·스킬이 동일한 단일 공격력 채널을 소비한다(스킬 계수 경로도 같은 Attack 기반).</summary>
        [Test]
        public void Test_S3_SkillPathConsumesSameSingleAttackChannel()
        {
            const float attackPower = 100f;
            const float skillCoefficient = 1.5f;
            var attacker = MakeAttacker(attack: attackPower);
            var victim = MakeVictim(defense: 0f);

            float result = DamageCalculator.Calculate(attacker, victim, isCritical: false, skillCoefficient: skillCoefficient);

            // 단일 공격력 100 * 스킬계수 1.5 = 150
            Assert.That(result, Is.EqualTo(150f));
        }

        // ── DoD-S7: 받는 피해 감소 단계 제거, 입히는 피해 증가 유지 ──

        /// <summary>DoD-S7: 입히는 피해 증가 단계는 유지되어 데미지를 증가시킨다.</summary>
        [Test]
        public void Test_S7_DamageDealtIncreaseStillApplied()
        {
            const float attackPower = 100f;
            const float damageDealtIncrease = 0.2f;
            var attacker = MakeAttacker(attack: attackPower, damageDealtIncrease: damageDealtIncrease);
            var victim = MakeVictim(defense: 0f);

            float result = DamageCalculator.Calculate(attacker, victim, isCritical: false);

            // 100 * (1 + 0.2) = 120
            Assert.That(result, Is.EqualTo(120f));
        }

        /// <summary>
        /// DoD-S7: 받는 피해 감소 단계가 파이프라인에서 사라졌다.
        /// 전체 요인(치명타·방어력·피해증가)을 적용했을 때, 받는 피해 감소 곱(×0.9)이
        /// 적용되지 않은 값이 나와야 한다. (정정 전: ×0.9 포함 113, 정정 후: 미포함 126)
        /// </summary>
        [Test]
        public void Test_S7_NoDamageReductionStepInPipeline()
        {
            const float attackPower = 100f;
            const float critMultiplier = 1.5f;
            const float defenseRatio = 0.3f;
            const float damageDealtIncrease = 0.2f;
            var attacker = MakeAttacker(
                attack: attackPower,
                criticalDamageMultiplier: critMultiplier,
                damageDealtIncrease: damageDealtIncrease);
            var victim = MakeVictim(defense: defenseRatio);

            float result = DamageCalculator.Calculate(attacker, victim, isCritical: true);

            // 100 * 1.5(치명타) * (1-0.3)(방어) * (1+0.2)(피해증가) = 126 → floor 126
            // (받는 피해 감소 단계가 남아 있다면 × 0.9 가 더 적용되어 113 이 나온다.)
            Assert.That(result, Is.EqualTo(126f));
        }

        // ── DoD-S6: 스탯 정보 패널이 10종만 표시 ──

        /// <summary>
        /// DoD-S6: 스탯 정보 패널의 표시 집합(상단+하단 그리드)이 10종 능력치와 정확히 일치한다.
        /// 표시 구성은 private static 배열이므로 리플렉션으로 읽는다.
        /// </summary>
        [Test]
        public void Test_S6_StatInfoPanelDisplaysExactlyThe10Kinds()
        {
            var mainStats = ReadStaticKindArray("MainStats");
            var subStats = ReadStaticKindArray("SubStats");

            var displayedKinds = mainStats.Concat(subStats).ToArray();

            Assert.That(displayedKinds, Is.EquivalentTo(Enum.GetValues(typeof(UnitStatKind)).Cast<UnitStatKind>()));
        }

        /// <summary>DoD-S6: 패널 표시 집합에 중복 종류가 없다(10종을 한 번씩만 표시).</summary>
        [Test]
        public void Test_S6_StatInfoPanelHasNoDuplicateDisplayedKinds()
        {
            var mainStats = ReadStaticKindArray("MainStats");
            var subStats = ReadStaticKindArray("SubStats");

            var displayedKinds = mainStats.Concat(subStats).ToArray();

            Assert.That(displayedKinds, Is.Unique);
        }

        // ── 헬퍼 ──

        /// <summary>지정한 단일 공격력·치명타 배율·피해 증가를 가진 공격자 스탯 시트를 만든다.</summary>
        private static UnitStatSheet MakeAttacker(
            float attack,
            float criticalDamageMultiplier = 1.5f,
            float damageDealtIncrease = 0f)
        {
            var sheet = new UnitStatSheet();
            sheet.Attack.SetBaseValue(attack);
            sheet.CriticalChance.SetBaseValue(0f);
            sheet.CriticalDamageMultiplier.SetBaseValue(criticalDamageMultiplier);
            sheet.DamageDealtIncrease.SetBaseValue(damageDealtIncrease);
            return sheet;
        }

        /// <summary>지정한 단일 방어력을 가진 피격자 스탯 시트를 만든다.</summary>
        private static UnitStatSheet MakeVictim(float defense)
        {
            var sheet = new UnitStatSheet();
            sheet.Defense.SetBaseValue(defense);
            return sheet;
        }

        /// <summary>StatInfoPanel 의 private static UnitStatKind[] 표시 배열을 리플렉션으로 읽는다.</summary>
        private static IEnumerable<UnitStatKind> ReadStaticKindArray(string fieldName)
        {
            var field = typeof(StatInfoPanel).GetField(
                fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(field, Is.Not.Null, $"StatInfoPanel.{fieldName} 필드를 찾지 못함");
            return (UnitStatKind[])field.GetValue(null);
        }
    }
}
