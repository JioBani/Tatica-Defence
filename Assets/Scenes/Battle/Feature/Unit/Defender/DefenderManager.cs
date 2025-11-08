using System.Collections.Generic;
using System.Linq;
using Common.Data.Units.UnitLoadOuts;
using Scenes.Battle.Feature.Units;
using Scenes.Battle.Feature.Units.ActionStates;
using Scenes.Battle.Feature.WaitingAreas;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Defenders
{
    public class DefenderManager : MonoBehaviour
    {
        [SerializeField] private UnitGenerator unitGenerator;
        private List<Units.Unit> units = new List<Units.Unit>();

        public bool GenerateDefender(UnitLoadOutData unitLoadOutData)
        {
            //TODO: 대기석이 비어있지 않는지 확인하는 것을 여기서 하는것이 맞는지 고려 필요
            if (WaitingAreaReferences.Instance.waitingAreas.Find((zone) => zone.occupant == null))
            {
                var unit = unitGenerator.GenerateDefender(unitLoadOutData);
                
                unit.MoveToWaitingArea();
                
                units.Add(unit);

                return true;
            }
            else
            {
                Debug.Log("대기석이 가득찼습니다.");

                return false;
            }
        }

        public bool IsAllDefenderDowned()
        {
            return units.All((unit) => unit.ActionStateController.CurrentStateType == ActionStateType.Downed);
        }
    }
}
