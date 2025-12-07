using Scenes.Battle.Feature.Unit.Defenders;
using UnityEngine;

namespace Scenes.Battle.Feature.Markets
{
    public class DefenderSellZone : MonoBehaviour
    {
        private Camera _mainCamera;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _mainCamera = Camera.main;
        }

        public void TrySell(Defender defender)
        {
            bool isOverSellArea = RectTransformUtility.RectangleContainsScreenPoint(
                _rectTransform,
                _mainCamera.WorldToScreenPoint(defender.transform.position),
                null
            );
            
            if (isOverSellArea)
            {
                MarketManager.Instance.Sell(defender);
            }
        }
    }
}
