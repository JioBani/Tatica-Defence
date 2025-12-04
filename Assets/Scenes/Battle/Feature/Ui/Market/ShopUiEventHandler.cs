using Common.Data.Units.UnitLoadOuts;
using Scenes.Battle.Feature.Unit.Defenders;
using Scenes.Battle.Feature.Units;
using Scenes.Battle.Feature.WaitingAreas;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui.Shop
{
    public class ShopUiEventHandler : MonoBehaviour
    {
        [SerializeField] private Transform battleField;
        [SerializeField] private UnitGenerator unitGenerator;
        [SerializeField] private UnitLoadOutData tempUnitLoadOutData;
        [SerializeField] private DefenderManager defenderManager;
        
        public void OnClickDefenderBuyButton(int index)
        {
            //TODO: 대기석이 비어있지 않는지 확인하는 것을 여기서 하는것이 맞는지 고려 필요
            if (WaitingAreaReferences.Instance.waitingAreas.Find((zone) => zone.occupant == null))
            {
                defenderManager.GenerateDefender(tempUnitLoadOutData);
            }
            else
            {
                Debug.Log("대기석이 가득찼습니다.");
            }
        }
    }
}


