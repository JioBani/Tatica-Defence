using System.Collections.Generic;
using Common.Data.Synergies;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.GlobalEventBus;
using Scenes.Battle.Feature.Events;
using Scenes.Battle.Feature.Synergy;
using Scenes.Battle.Feature.Unit.Defenders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Battle.Feature.Ui.SynergyInfo
{
    /// <summary>
    /// 시너지 상세 패널. 시너지 클릭 시 상세 정보를 표시한다.
    /// </summary>
    public class SynergyDetailPanel : MonoBehaviour
    {
        /// <summary>시너지 아이콘.</summary>
        [SerializeField] private Image detailIcon;

        /// <summary>시너지 이름.</summary>
        [SerializeField] private TMP_Text detailName;

        /// <summary>닫기 버튼.</summary>
        [SerializeField] private Button closeButton;

        /// <summary>시너지 효과 설명.</summary>
        [SerializeField] private TMP_Text descriptionText;

        /// <summary>티어 행 컨테이너.</summary>
        [SerializeField] private Transform tierListContainer;

        /// <summary>티어 행 프리팹.</summary>
        [SerializeField] private SynergyTierRow tierRowPrefab;

        /// <summary>소환수 슬롯 컨테이너.</summary>
        [SerializeField] private Transform unitListContainer;

        /// <summary>소환수 슬롯 프리팹.</summary>
        [SerializeField] private SynergyUnitSlot unitSlotPrefab;

        /// <summary>Defender 목록 접근용.</summary>
        [SerializeField] private DefenderManager defenderManager;

        /// <summary>현재 표시 중인 시너지. 토글 판정에 사용한다.</summary>
        private SynergyActivation _currentActivation;

        /// <summary>현재 표시 중인 시너지.</summary>
        public SynergyActivation CurrentActivation => _currentActivation;

        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
        }

        private void OnEnable()
        {
            GlobalEventBus.Subscribe<OnDefenderPlacementChangedEventDto>(HandlePlacementChanged);
            GlobalEventBus.Subscribe<OnDefenderChangedEventDto>(HandleDefenderChanged);
        }

        private void OnDisable()
        {
            GlobalEventBus.Unsubscribe<OnDefenderPlacementChangedEventDto>(HandlePlacementChanged);
            GlobalEventBus.Unsubscribe<OnDefenderChangedEventDto>(HandleDefenderChanged);
        }

        /// <summary>상세 패널을 열고 시너지 데이터를 바인딩한다.</summary>
        public void Show(SynergyActivation activation)
        {
            _currentActivation = activation;
            detailIcon.sprite = activation.Definition.Icon;
            detailName.text = activation.Definition.DisplayName;
            descriptionText.text = activation.Definition.Description;

            BindTierRows(activation);
            BindUnitSlots(activation);

            gameObject.SetActive(true);
        }

        /// <summary>상세 패널을 닫는다.</summary>
        public void Hide()
        {
            _currentActivation = null;
            gameObject.SetActive(false);
        }

        /// <summary>티어 행을 동적 생성하여 바인딩한다.</summary>
        private void BindTierRows(SynergyActivation activation)
        {
            // 기존 티어 행을 모두 파괴한다
            for (int i = tierListContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(tierListContainer.GetChild(i).gameObject);
            }

            IReadOnlyList<SynergyTier> tiers = activation.Definition.Tiers;
            SynergyTier? activeTier = activation.ActiveTier.Value;

            for (int i = 0; i < tiers.Count; i++)
            {
                SynergyTierRow row = Instantiate(tierRowPrefab, tierListContainer);
                bool isActive = activeTier.HasValue && activeTier.Value.Tier == tiers[i].Tier;
                row.Bind(tiers[i], isActive);
            }
        }

        /// <summary>소환수 슬롯을 동적 생성하여 바인딩한다.</summary>
        private void BindUnitSlots(SynergyActivation activation)
        {
            // 기존 슬롯을 모두 파괴한다
            for (int i = unitListContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(unitListContainer.GetChild(i).gameObject);
            }

            List<UnitLoadOutData> units = SynergyManager.Instance.GetUnitsForSynergy(activation.Definition);

            for (int i = 0; i < units.Count; i++)
            {
                SynergyUnitSlot slot = Instantiate(unitSlotPrefab, unitListContainer);
                slot.Bind(units[i]);
            }

            RefreshUnitSlotStates();
        }

        /// <summary>소환수 슬롯의 배치 상태를 갱신한다.</summary>
        private void RefreshUnitSlotStates()
        {
            if (_currentActivation == null)
            {
                return;
            }

            for (int i = 0; i < unitListContainer.childCount; i++)
            {
                SynergyUnitSlot slot = unitListContainer.GetChild(i).GetComponent<SynergyUnitSlot>();
                if (slot != null)
                {
                    bool isDeployed = IsUnitDeployed(slot.UnitLoadOut);
                    slot.SetDeployed(isDeployed);
                }
            }
        }

        /// <summary>해당 소환수가 전장에 배치되어 있는지 판별한다.</summary>
        private bool IsUnitDeployed(UnitLoadOutData unitLoadOut)
        {
            IReadOnlyList<Defender> defenders = defenderManager.Defenders;

            for (int i = 0; i < defenders.Count; i++)
            {
                if (defenders[i].UnitLoadOutData == unitLoadOut && defenders[i].Placement == Placement.BattleArea)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Defender 배치 변경 시 슬롯 상태를 갱신한다.</summary>
        private void HandlePlacementChanged(OnDefenderPlacementChangedEventDto dto)
        {
            RefreshUnitSlotStates();
        }

        /// <summary>Defender 스폰/디스폰 시 슬롯 상태를 갱신한다.</summary>
        private void HandleDefenderChanged(OnDefenderChangedEventDto dto)
        {
            RefreshUnitSlotStates();
        }
    }
}
