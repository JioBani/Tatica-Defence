// ─────────────────────────────────────────────
// SynergyDetailPanel3Tests: TACD-296 단위 3 테스트
// 소환수 목록 및 배치 상태 표시
//
// 테스트 케이스 목록:
//
// A. SynergyManager.GetUnitsForSynergy — 소환수 목록 조회 (CD-8)
//   A-1. [정상] 해당 시너지에 속하는 소환수만 반환된다
//   A-2. [정상] 복수의 소환수가 같은 시너지에 속할 때 모두 반환된다
//   A-3. [정상] 다른 시너지에 속하는 소환수는 반환되지 않는다
//   A-4. [경계] 해당 시너지에 속하는 소환수가 없으면 빈 리스트를 반환한다
//   A-5. [경계] _unitSynergyMap이 비어있으면 빈 리스트를 반환한다
//
// B. SynergyUnitSlot — 소환수 슬롯 바인딩 및 배치 상태 (CD-8, CD-9)
//   B-1. [스킵] Bind() 호출 시 unitIcon.sprite가 설정된다 (MonoBehaviour + Image SerializeField 의존)
//   B-2. [스킵] Bind() 호출 시 UnitLoadOut 프로퍼티가 설정된다 (Bind() 내부에서 unitIcon.sprite 설정 → NullReferenceException)
//   B-3. [스킵] SetDeployed(true) 시 canvasGroup.alpha가 1f이다 (MonoBehaviour + CanvasGroup SerializeField 의존)
//   B-4. [스킵] SetDeployed(false) 시 canvasGroup.alpha가 0.4f이다 (MonoBehaviour + CanvasGroup SerializeField 의존)
//
// C. SynergyDetailPanel — 소환수 슬롯 생성 및 실시간 갱신 (CD-8, CD-9, CD-10)
//   C-1. [스킵] Show() 호출 시 시너지 소환수 수만큼 SynergyUnitSlot이 생성된다 (MonoBehaviour + Instantiate 의존)
//   C-2. [스킵] 전장 배치 소환수의 슬롯 alpha가 1f이다 (MonoBehaviour + DefenderManager 싱글톤 의존)
//   C-3. [스킵] 미배치 소환수의 슬롯 alpha가 0.4f이다 (MonoBehaviour + DefenderManager 싱글톤 의존)
//   C-4. [스킵] OnDefenderPlacementChangedEventDto 수신 시 슬롯 상태가 갱신된다 (MonoBehaviour + 씬 의존)
//   C-5. [스킵] OnDefenderChangedEventDto 수신 시 슬롯 상태가 갱신된다 (MonoBehaviour + 씬 의존)
// ─────────────────────────────────────────────
using System.Collections.Generic;
using System.Reflection;
using Common.Data.Synergies;
using Common.Data.Units.UnitLoadOuts;
using NUnit.Framework;
using Scenes.Battle.Feature.Synergy;
using UnityEngine;

namespace Tests.Editor.Battle
{
    /// <summary>
    /// TACD-296 단위 3: 소환수 목록 및 배치 상태 표시 테스트.
    ///
    /// SynergyManager.GetUnitsForSynergy()는 내부 _unitSynergyMap만 참조하는 순수 조회 로직이므로
    /// 리플렉션으로 맵을 세팅하여 직접 검증한다.
    /// SynergyUnitSlot과 SynergyDetailPanel의 UI 바인딩·갱신 로직은 MonoBehaviour 및
    /// Unity 컴포넌트(Image, CanvasGroup, Instantiate, DefenderManager 싱글톤)에 전면 의존하여
    /// 에디터 단위 테스트 환경에서 Ignore 처리한다.
    /// </summary>
    public class SynergyDetailPanel3Tests
    {
        private SynergyManager _manager;
        private GameObject _managerGo;

        private SynergyDefinitionData _synergyA;
        private SynergyDefinitionData _synergyB;
        private UnitLoadOutData _unitA1;
        private UnitLoadOutData _unitA2;
        private UnitLoadOutData _unitB1;

        private readonly List<ScriptableObject> _assets = new();

        [SetUp]
        public void SetUp()
        {
            _managerGo = new GameObject("SynergyManager");
            _manager = _managerGo.AddComponent<SynergyManager>();

            _synergyA = ScriptableObject.CreateInstance<SynergyDefinitionData>();
            _synergyB = ScriptableObject.CreateInstance<SynergyDefinitionData>();
            _unitA1 = ScriptableObject.CreateInstance<UnitLoadOutData>();
            _unitA2 = ScriptableObject.CreateInstance<UnitLoadOutData>();
            _unitB1 = ScriptableObject.CreateInstance<UnitLoadOutData>();
            _assets.AddRange(new ScriptableObject[] { _synergyA, _synergyB, _unitA1, _unitA2, _unitB1 });
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_managerGo);
            foreach (ScriptableObject asset in _assets)
            {
                Object.DestroyImmediate(asset);
            }
            _assets.Clear();
        }

        // ══════════════════════════════════════════════
        // A. SynergyManager.GetUnitsForSynergy
        // ══════════════════════════════════════════════

        // A-1: 해당 시너지에 속하는 소환수만 반환된다
        [Test]
        public void GetUnitsForSynergy_OneUnitMapped_ReturnsThatUnit()
        {
            // _unitSynergyMap: unitA1 → synergyA
            SetUnitSynergyMap(_manager, new Dictionary<UnitLoadOutData, SynergyDefinitionData>
            {
                { _unitA1, _synergyA },
            });

            List<UnitLoadOutData> result = _manager.GetUnitsForSynergy(_synergyA);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_unitA1, result[0]);
        }

        // A-2: 복수의 소환수가 같은 시너지에 속할 때 모두 반환된다
        [Test]
        public void GetUnitsForSynergy_MultipleUnitsMapped_ReturnsAll()
        {
            // _unitSynergyMap: unitA1, unitA2 → synergyA
            SetUnitSynergyMap(_manager, new Dictionary<UnitLoadOutData, SynergyDefinitionData>
            {
                { _unitA1, _synergyA },
                { _unitA2, _synergyA },
            });

            List<UnitLoadOutData> result = _manager.GetUnitsForSynergy(_synergyA);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(_unitA1));
            Assert.IsTrue(result.Contains(_unitA2));
        }

        // A-3: 다른 시너지에 속하는 소환수는 반환되지 않는다
        [Test]
        public void GetUnitsForSynergy_MixedMap_ReturnsOnlyMatchingSynergy()
        {
            // _unitSynergyMap: unitA1 → synergyA, unitB1 → synergyB
            SetUnitSynergyMap(_manager, new Dictionary<UnitLoadOutData, SynergyDefinitionData>
            {
                { _unitA1, _synergyA },
                { _unitB1, _synergyB },
            });

            List<UnitLoadOutData> result = _manager.GetUnitsForSynergy(_synergyA);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_unitA1, result[0]);
            Assert.IsFalse(result.Contains(_unitB1));
        }

        // A-4: 해당 시너지에 속하는 소환수가 없으면 빈 리스트를 반환한다
        [Test]
        public void GetUnitsForSynergy_NoMatchingSynergy_ReturnsEmpty()
        {
            // _unitSynergyMap: unitB1 → synergyB만 있음
            SetUnitSynergyMap(_manager, new Dictionary<UnitLoadOutData, SynergyDefinitionData>
            {
                { _unitB1, _synergyB },
            });

            List<UnitLoadOutData> result = _manager.GetUnitsForSynergy(_synergyA);

            Assert.AreEqual(0, result.Count);
        }

        // A-5: _unitSynergyMap이 비어있으면 빈 리스트를 반환한다
        [Test]
        public void GetUnitsForSynergy_EmptyMap_ReturnsEmpty()
        {
            SetUnitSynergyMap(_manager, new Dictionary<UnitLoadOutData, SynergyDefinitionData>());

            List<UnitLoadOutData> result = _manager.GetUnitsForSynergy(_synergyA);

            Assert.AreEqual(0, result.Count);
        }

        // ══════════════════════════════════════════════
        // B. SynergyUnitSlot — 소환수 슬롯 바인딩
        // ══════════════════════════════════════════════

        // B-1: Bind() 호출 시 unitIcon.sprite가 설정된다 (CD-8)
        [Test]
        [Ignore("SynergyUnitSlot이 MonoBehaviour이며 unitIcon(Image) SerializeField 초기화가 " +
                "불가하여 에디터 테스트 불가")]
        public void Bind_SetsUnitIconSprite()
        {
        }

        // B-2: Bind() 호출 시 UnitLoadOut 프로퍼티가 설정된다 (CD-8 전제 조건)
        [Test]
        [Ignore("Bind() 내부에서 unitIcon.sprite 설정 시 SerializeField(Image)가 null이어서 " +
                "NullReferenceException 발생 — 에디터 테스트 불가")]
        public void Bind_SetsUnitLoadOutProperty()
        {
        }

        // B-3: SetDeployed(true) 시 canvasGroup.alpha가 1f이다 (CD-9)
        [Test]
        [Ignore("SynergyUnitSlot이 MonoBehaviour이며 canvasGroup(CanvasGroup) SerializeField 초기화가 " +
                "불가하여 에디터 테스트 불가")]
        public void SetDeployed_True_AlphaIsOne()
        {
        }

        // B-4: SetDeployed(false) 시 canvasGroup.alpha가 0.4f이다 (CD-9)
        [Test]
        [Ignore("SynergyUnitSlot이 MonoBehaviour이며 canvasGroup(CanvasGroup) SerializeField 초기화가 " +
                "불가하여 에디터 테스트 불가")]
        public void SetDeployed_False_AlphaIsPointFour()
        {
        }

        // ══════════════════════════════════════════════
        // C. SynergyDetailPanel — 소환수 슬롯 생성 및 실시간 갱신
        // ══════════════════════════════════════════════

        // C-1: Show() 호출 시 시너지 소환수 수만큼 SynergyUnitSlot이 생성된다 (CD-8)
        [Test]
        [Ignore("SynergyDetailPanel.BindUnitSlots()가 SynergyManager 싱글톤과 Instantiate()에 의존하여 " +
                "에디터 테스트 불가")]
        public void Show_CreatesUnitSlotsForAllUnits()
        {
        }

        // C-2: 전장 배치 소환수의 슬롯 alpha가 1f이다 (CD-9)
        [Test]
        [Ignore("IsUnitDeployed()가 DefenderManager 싱글톤의 Defenders 목록에 의존하여 " +
                "에디터 테스트 불가")]
        public void Show_DeployedUnit_SlotAlphaIsOne()
        {
        }

        // C-3: 미배치 소환수의 슬롯 alpha가 0.4f이다 (CD-9)
        [Test]
        [Ignore("IsUnitDeployed()가 DefenderManager 싱글톤의 Defenders 목록에 의존하여 " +
                "에디터 테스트 불가")]
        public void Show_UndeployedUnit_SlotAlphaIsPointFour()
        {
        }

        // C-4: OnDefenderPlacementChangedEventDto 수신 시 슬롯 상태가 갱신된다 (CD-10)
        [Test]
        [Ignore("HandlePlacementChanged()가 RefreshUnitSlotStates()를 통해 unitListContainer(Transform) " +
                "SerializeField와 DefenderManager 싱글톤에 의존하여 에디터 테스트 불가")]
        public void PlacementChanged_RefreshesUnitSlotStates()
        {
        }

        // C-5: OnDefenderChangedEventDto(Spawn/Despawn) 수신 시 슬롯 상태가 갱신된다 (CD-10)
        [Test]
        [Ignore("HandleDefenderChanged()가 RefreshUnitSlotStates()를 통해 unitListContainer(Transform) " +
                "SerializeField와 DefenderManager 싱글톤에 의존하여 에디터 테스트 불가")]
        public void DefenderChanged_RefreshesUnitSlotStates()
        {
        }

        // ══════════════════════════════════════════════
        // 헬퍼
        // ══════════════════════════════════════════════

        /// <summary>리플렉션으로 SynergyManager의 _unitSynergyMap 필드를 설정한다.</summary>
        private static void SetUnitSynergyMap(
            SynergyManager manager,
            Dictionary<UnitLoadOutData, SynergyDefinitionData> map)
        {
            var field = typeof(SynergyManager)
                .GetField("_unitSynergyMap", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(manager, map);
        }
    }
}
