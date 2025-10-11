using Common.Scripts.ObjectPool;
using Scenes.Battle.Scripts.ObjectPool;
using Scenes.Battle.Scripts.WaitingArea;
using UnityEngine;

namespace Scenes.Battle.Scripts.Ui.Shop
{
    public class ShopUiEventHandler : MonoBehaviour
    {
        [SerializeField] private Transform battleField;
        
        public void OnClickDefenderBuyButton(int index)
        {
            if (WaitingAreaReferences.Instance.waitingAreas.Find((zone) => zone.occupant == null))
            {
                ObjectPooler.Instance.Spawn(ObjectPoolingReferences.Instance.unitPrefab, battleField);
            }
            else
            {
                Debug.Log("대기석이 가득찼습니다.");
            }
        }
    }
}


