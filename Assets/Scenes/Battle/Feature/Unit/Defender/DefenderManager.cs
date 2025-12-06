using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.SceneSingleton;
using Scenes.Battle.Feature.Units;
using Scenes.Battle.Feature.Units.ActionStates;
using Scenes.Battle.Feature.WaitingAreas;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Defenders
{
    public class DefenderManager : SceneSingleton<DefenderManager>
    {
        [SerializeField] private UnitGenerator unitGenerator;
        private List<Defender> units = new List<Defender>();
        public Action<Defender, Placement> OnPlacementChange;

        public int GetPlacementCount(Placement placement)
        {
            return units.Count(defender => defender.Placement == placement);
        }

        public bool GenerateDefender(UnitLoadOutData unitLoadOutData)
        {
            //TODO: 대기석이 비어있지 않는지 확인하는 것을 여기서 하는것이 맞는지 고려 필요
            if (WaitingAreaReferences.Instance.waitingAreas.Find((zone) => zone.occupant == null))
            {
                var unit = unitGenerator.GenerateDefender(unitLoadOutData);
                
                unit.MoveToWaitingArea();
                
                units.Add(unit.GetComponent<Defender>());

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

        public void RecordPlacement(Defender defender, Placement placement)
        {
            OnPlacementChange?.Invoke(defender, placement);
        }
    }
}
