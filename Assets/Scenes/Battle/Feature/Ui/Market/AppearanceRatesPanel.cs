using System;
using System.Collections.Generic;
using Scenes.Battle.Feature.Markets;
using TMPro;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui.Markets
{
    // 유닛 등장 확률 패널
    public class AppearanceRatesPanel : MonoBehaviour
    {
        private MarketUnitRoller _roller;
        [SerializeField] private List<TextMeshProUGUI> probabilityTextList;

        private void Start()
        {
            _roller = MarketManager.Instance.Roller;
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            var probability = _roller.ProbabilityByCost;
            for (var i = 0; i < probabilityTextList.Count; i++)
            {
                probabilityTextList[i].text = $"{probability[i + 1]}%";
            }
        }
    }
}
