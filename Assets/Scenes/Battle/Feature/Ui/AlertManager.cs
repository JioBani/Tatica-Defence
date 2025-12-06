using System;
using Scenes.Battle.Feature.Markets;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui
{
    public class AlertManager : MonoBehaviour
    {
        private void OnEnable()
        {
            MarketManager.Instance.OnGoldNotEnough += OnGoldNotEnough;
        }

        private void OnDisable()
        {
            MarketManager.Instance.OnGoldNotEnough -= OnGoldNotEnough;
        }

        private void OnDestroy()
        {
            MarketManager.Instance.OnGoldNotEnough -= OnGoldNotEnough;
        }

        void OnGoldNotEnough(OnGoldNotEnoughDto dto)
        {
            Debug.Log("Gold not enough");
        }
    }
}
