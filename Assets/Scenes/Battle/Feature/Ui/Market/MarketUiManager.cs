using Scenes.Battle.Feature.Markets;
using TMPro;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui.Markets
{
    public class MarketUiManager : MonoBehaviour
    {
        [SerializeField] private MarketManager marketManager;
        [SerializeField] private TextMeshProUGUI goldText;

        private void OnEnable()
        {
            marketManager.Gold.OnChange += OnGoldChange;
        }

        private void OnDisable()
        {
            marketManager.Gold.OnChange -= OnGoldChange;
        }

        private void OnDestroy()
        {
            marketManager.Gold.OnChange -= OnGoldChange;
        }

        private void OnGoldChange(int gold)
        {
            goldText.text = $"{gold} GOLD";
        }
    }
}
