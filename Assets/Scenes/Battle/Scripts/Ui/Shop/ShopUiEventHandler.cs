using Common.ObjectPool;
using Scenes.Battle.Scripts.ObjectPool;
using UnityEngine;

namespace Scenes.Battle.Scripts.Ui.Shop
{
    public class ShopUiEventHandler : MonoBehaviour
    {
        [SerializeField] private Transform battleField;
        
        public void OnClickDefenderBuyButton(int index)
        {
            ObjectPooler.Instance.Spawn(ObjectPoolingReferences.Instance.unitPrefab, battleField);
        }
    }
}


