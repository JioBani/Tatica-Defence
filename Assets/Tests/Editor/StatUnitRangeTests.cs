using System.Collections.Generic;
using Common.Data.Units.UnitStatsByLevel;
using NUnit.Framework;
using Scenes.Battle.Feature.Ui.StatInfoPanel;
using Scenes.Battle.Feature.Units.UnitStats;
using Scenes.Battle.Feature.Units.UnitStats.UnitStatSheets;
using UnityEngine;

namespace Tests.Editor
{
    /// <summary>
    /// 구현 단위 stat-unit-range (Task-3, SR 그룹 B / DoD-S8~S10) 의 요구사항 테스트.
    /// 능력치 단위·범위·기본값의 단일 진실 정의 + 최종값 강제(범위 클램프·정수화) + 표시 정합을 검증한다.
    /// 검증 기준은 UR Req-3 / SR DoD-S8~S10 의 완료 정의(정정 반영본: 치명타 배율 = 배수 1.0~∞ 기본 1.5)이며,
    /// 구현 코드 동작이 아니다.
    /// </summary>
    public class StatUnitRangeTests
    {
        private UnitStatMetadataCatalog _catalog;

        [SetUp]
        public void SetUp()
        {
            _catalog = new UnitStatMetadataCatalog();
        }

        // ── DoD-S8: 각 능력치의 단위·범위가 단일 진실로 정의된다 ──

        /// <summary>
        /// DoD-S8: 단일 진실 카탈로그가 각 능력치 종류의 단위·범위(min~max)를 요구사항 표대로 정의한다.
        /// (% 단위는 0~1 저장, 무제한 상한은 +∞.)
        /// </summary>
        [TestCaseSource(nameof(MetadataDefinitionCases))]
        public void Test_S8_CatalogDefinesUnitAndRangePerRequirement(
            UnitStatKind kind, StatUnit expectedUnit, float expectedMin, float expectedMax)
        {
            UnitStatMetadata metadata = _catalog.Get(kind);

            Assert.That(metadata.Unit, Is.EqualTo(expectedUnit), "단위");
            Assert.That(metadata.Min, Is.EqualTo(expectedMin), "하한");
            Assert.That(metadata.Max, Is.EqualTo(expectedMax), "상한");
        }

        // 요구사항(DoD-S8 정정 반영본) 의 단위·범위 표.
        private static IEnumerable<TestCaseData> MetadataDefinitionCases()
        {
            yield return new TestCaseData(UnitStatKind.MaxHealth, StatUnit.Integer, 1f, float.PositiveInfinity).SetName("최대체력 정수 1~무한");
            yield return new TestCaseData(UnitStatKind.Attack, StatUnit.Integer, 0f, float.PositiveInfinity).SetName("공격력 정수 0~무한");
            yield return new TestCaseData(UnitStatKind.Defense, StatUnit.Percent, 0f, 0.95f).SetName("방어력 % 0~95%");
            yield return new TestCaseData(UnitStatKind.AttackSpeed, StatUnit.Float, 0.1f, float.PositiveInfinity).SetName("공격속도 실수 0.1~무한");
            yield return new TestCaseData(UnitStatKind.AttackRange, StatUnit.Float, 0f, float.PositiveInfinity).SetName("사거리 실수 0~무한");
            yield return new TestCaseData(UnitStatKind.MoveSpeed, StatUnit.Float, 0f, float.PositiveInfinity).SetName("이동속도 실수 0~무한");
            yield return new TestCaseData(UnitStatKind.CriticalChance, StatUnit.Percent, 0f, 1f).SetName("치명타확률 % 0~100%");
            yield return new TestCaseData(UnitStatKind.CriticalDamageMultiplier, StatUnit.Multiplier, 1f, float.PositiveInfinity).SetName("치명타배율 배수 1.0~무한");
            yield return new TestCaseData(UnitStatKind.CooldownReduction, StatUnit.Percent, 0f, 0.9f).SetName("쿨타임감소 % 0~90%");
            yield return new TestCaseData(UnitStatKind.DamageDealtIncrease, StatUnit.Percent, 0f, float.PositiveInfinity).SetName("피해증가 % 0~무한");
        }

        // ── DoD-S8: 자산 미입력 시 요구사항이 명시한 기본값으로 fallback ──

        /// <summary>
        /// DoD-S8: 성급 기준값 자산에 값이 미입력이면 요구사항이 명시한 기본값을 쓴다.
        /// 치명타 배율 기본 1.5(미입력이 0→클램프 1.0이 아니라 1.5), 치명타 확률 기본 0.
        /// </summary>
        [TestCase(UnitStatKind.CriticalDamageMultiplier, 1.5f)]
        [TestCase(UnitStatKind.CriticalChance, 0f)]
        public void Test_S8_MissingAssetValueFallsBackToRequirementDefault(UnitStatKind kind, float expectedDefault)
        {
            var emptyData = ScriptableObject.CreateInstance<UnitStatsByLevelData>();
            var sheet = new UnitStatSheet();

            sheet.Init(emptyData);

            Assert.That(sheet.Get(kind).CurrentValue, Is.EqualTo(expectedDefault).Within(0.001f));

            Object.DestroyImmediate(emptyData);
        }

        // ── DoD-S9: 최종값이 범위 상한을 넘지 않는다 (경계값) ──

        /// <summary>DoD-S9: 상한이 있는 능력치의 최종값이 상한을 넘지 않고 경계값으로 클램프된다.</summary>
        [TestCase(UnitStatKind.Defense, 0.99f, 0.95f)]            // 방어력 ≤ 95%
        [TestCase(UnitStatKind.CooldownReduction, 0.95f, 0.9f)]   // 쿨감 ≤ 90%
        [TestCase(UnitStatKind.CriticalChance, 1.5f, 1.0f)]       // 치명타확률 ≤ 100%
        public void Test_S9_FinalValueClampedToUpperBound(UnitStatKind kind, float rawBase, float expectedFinal)
        {
            var stat = new UnitStat(_catalog.Get(kind));

            stat.SetBaseValue(rawBase);

            Assert.That(stat.CurrentValue, Is.EqualTo(expectedFinal).Within(0.001f));
        }

        // ── DoD-S9: 최종값이 범위 하한 아래로 내려가지 않는다 (경계값) ──

        /// <summary>DoD-S9: 하한이 있는 능력치의 최종값이 하한 아래로 내려가지 않고 경계값으로 클램프된다.</summary>
        [TestCase(UnitStatKind.CriticalDamageMultiplier, 0.5f, 1.0f)] // 치명타배율 ≥ 1.0 (치명타가 일반보다 약할 수 없음)
        [TestCase(UnitStatKind.AttackSpeed, 0.05f, 0.1f)]            // 공격속도 ≥ 0.1
        [TestCase(UnitStatKind.MaxHealth, 0f, 1f)]                  // 최대체력 ≥ 1
        [TestCase(UnitStatKind.Attack, -5f, 0f)]                    // 공격력 ≥ 0
        [TestCase(UnitStatKind.Defense, -0.1f, 0f)]                 // 방어력 ≥ 0
        public void Test_S9_FinalValueClampedToLowerBound(UnitStatKind kind, float rawBase, float expectedFinal)
        {
            var stat = new UnitStat(_catalog.Get(kind));

            stat.SetBaseValue(rawBase);

            Assert.That(stat.CurrentValue, Is.EqualTo(expectedFinal).Within(0.001f));
        }

        // ── DoD-S9: 정수 단위 능력치의 최종값은 정수 ──

        /// <summary>DoD-S9: 정수 단위 능력치(공격력·최대 체력)의 최종값이 정수로 만들어진다.</summary>
        [TestCase(UnitStatKind.Attack, 10.6f, 11f)]
        [TestCase(UnitStatKind.Attack, 10.4f, 10f)]
        [TestCase(UnitStatKind.MaxHealth, 100.7f, 101f)]
        public void Test_S9_IntegerUnitFinalValueIsInteger(UnitStatKind kind, float rawBase, float expectedFinal)
        {
            var stat = new UnitStat(_catalog.Get(kind));

            stat.SetBaseValue(rawBase);

            Assert.That(stat.CurrentValue, Is.EqualTo(expectedFinal));
        }

        /// <summary>DoD-S9: 실수 단위 능력치(공격 속도 등)는 정수화하지 않고 소수 최종값을 유지한다.</summary>
        [Test]
        public void Test_S9_FloatUnitFinalValueNotRounded()
        {
            const float fractionalSpeed = 1.25f;
            var attackSpeed = new UnitStat(_catalog.Get(UnitStatKind.AttackSpeed));

            attackSpeed.SetBaseValue(fractionalSpeed);

            Assert.That(attackSpeed.CurrentValue, Is.EqualTo(fractionalSpeed).Within(0.0001f));
        }

        /// <summary>
        /// DoD-S9: 수정자가 붙어 산정된 최종값에도 범위 강제가 적용된다.
        /// 방어력 base 0에 +99% 수정자를 더해도 최종값은 상한 95%로 클램프된다.
        /// </summary>
        [Test]
        public void Test_S9_RangeEnforcedOnModifiedFinalValue()
        {
            var defense = new UnitStat(_catalog.Get(UnitStatKind.Defense));
            defense.SetBaseValue(0f);

            defense.AddModifier(new StatModifier("buff", StatModifierType.Flat, 0.99f));

            Assert.That(defense.CurrentValue, Is.EqualTo(0.95f).Within(0.001f));
        }

        // ── DoD-S10: 표시가 값 강제와 동일한 단위 정의를 따른다 ──

        /// <summary>
        /// DoD-S10: 스탯 표시가 단일 진실 정의의 단위를 읽어 표기한다.
        /// 방어력·치명타확률·쿨감은 %, 치명타배율은 "Nx", 사거리·이동속도는 소수, 정수 단위는 정수.
        /// </summary>
        [TestCase(UnitStatKind.Defense, 0.3f, "30%")]
        [TestCase(UnitStatKind.CriticalChance, 0.25f, "25%")]
        [TestCase(UnitStatKind.CooldownReduction, 0.1f, "10%")]
        [TestCase(UnitStatKind.CriticalDamageMultiplier, 1.5f, "1.5x")]
        [TestCase(UnitStatKind.AttackRange, 3.5f, "3.50")]
        [TestCase(UnitStatKind.MoveSpeed, 2f, "2.00")]
        [TestCase(UnitStatKind.MaxHealth, 1000f, "1000")]
        [TestCase(UnitStatKind.Attack, 150f, "150")]
        public void Test_S10_DisplayUsesUnitFromSingleSourceDefinition(UnitStatKind kind, float value, string expected)
        {
            StatUnit unit = _catalog.Get(kind).Unit;

            string formatted = unit.FormatStatValue(value);

            Assert.That(formatted, Is.EqualTo(expected));
        }

        /// <summary>
        /// DoD-S10: 값 강제(UnitStat.Metadata)와 표시가 같은 정의를 공유한다.
        /// 스탯 시트가 생성한 각 능력치의 메타데이터가 단일 진실 카탈로그와 일치한다.
        /// </summary>
        [Test]
        public void Test_S10_SheetStatsCarrySameMetadataAsCatalog()
        {
            var sheet = new UnitStatSheet();

            foreach (var (kind, stat) in sheet.Enumerate())
            {
                UnitStatMetadata catalogMeta = _catalog.Get(kind);
                Assert.That(stat.Metadata.Unit, Is.EqualTo(catalogMeta.Unit), $"{kind} 단위 불일치");
                Assert.That(stat.Metadata.Min, Is.EqualTo(catalogMeta.Min), $"{kind} 하한 불일치");
                Assert.That(stat.Metadata.Max, Is.EqualTo(catalogMeta.Max), $"{kind} 상한 불일치");
            }
        }
    }
}
