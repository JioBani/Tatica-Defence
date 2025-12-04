using System;
using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Markets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Battle.Feature.Ui.Markets
{
    public class MarketUiManager : MonoBehaviour
    {
        [SerializeField] private MarketManager marketManager;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private List<Image> defenderSlotImageList;

        private void OnEnable()
        {
            marketManager.Gold.OnChange += OnGoldChange;
            marketManager.OnSlotRerolled += OnSlotRerolled;
        }

        private void OnDisable()
        {
            marketManager.Gold.OnChange -= OnGoldChange;
            marketManager.OnSlotRerolled -= OnSlotRerolled;
        }

        private void OnDestroy()
        {
            marketManager.Gold.OnChange -= OnGoldChange;
            marketManager.OnSlotRerolled -= OnSlotRerolled;
        }

        private void OnGoldChange(int gold)
        {
            goldText.text = $"{gold} GOLD";
        }

        private void OnSlotRerolled(List<UnitLoadOutData> units)
        {
            for (int i = 0; i < units.Count; i++)
            {
                defenderSlotImageList[i].sprite = units[i].Unit.Illustration;
            }
        }

        public void OnClickSlot(int index)
        {
            MarketManager.Instance.BuyDefender(index);
        }
    }
}
