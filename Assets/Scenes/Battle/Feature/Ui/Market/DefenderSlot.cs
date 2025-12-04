using System;
using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using Scenes.Battle.Feature.Unit.Defenders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Battle.Feature.Markets
{
    /// <summary>
    /// 상점 UI 의 수호자 구매 슬롯
    /// </summary>
    public class DefenderSlot : MonoBehaviour
    {
        private MarketManager _marketManager;
        [SerializeField] private DefenderManager defenderManager;
        
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI purchasedText;
        
        [SerializeField] private int index;
        
        private UnitLoadOutData _unitData;
        private bool IsPurchased { get; set; } = false;

        private void Awake()
        {
            _marketManager = MarketManager.Instance;
        }

        private void OnEnable()
        {
            _marketManager.OnSlotRerolled += OnSlotRerolled;
        }

        private void OnDisable()
        {
            _marketManager.OnSlotRerolled -= OnSlotRerolled;
        }

        private void OnDestroy()
        {
            _marketManager.OnSlotRerolled -= OnSlotRerolled;
        }

        private void OnSlotRerolled(List<UnitLoadOutData> unitDataList)
        {
            // 자신의 슬롯 번호에 해당하는 유닛 데이터 장착
            SetUnitData(unitDataList[index]);
        }

        private void SetUnitData(UnitLoadOutData unitData)
        {
            _unitData = unitData;
            image.sprite = unitData.Unit.Illustration;
            ActivateImage();
        }

        public void OnClick()
        {
            if (!IsPurchased)
            {
                Purchase();
            }
        }

        private void Purchase()
        {
            if (defenderManager.GenerateDefender(_unitData))
            {
                IsPurchased = true;
                DeactivateImage();
            }
        }

        private void DeactivateImage()
        {
            image.gameObject.SetActive(false);
            purchasedText.gameObject.SetActive(true);
        }
        
        private void ActivateImage()
        {
            image.gameObject.SetActive(true);
            purchasedText.gameObject.SetActive(false);
        }
    }
}
