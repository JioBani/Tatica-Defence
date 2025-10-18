using System;
using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.Draggable;
using Common.Scripts.Enums;
using Scenes.Battle.Scripts.Unit.HUDs.HealthBar;
using Scenes.Battle.Scripts.Unit.UnitStats;
using Scenes.Battle.Scripts.WaitingArea;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit
{
    // TODO: 기능이 많아지는 경우 분리
    public class Unit : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;
        
        private Draggable2D _draggable;
        // private UnitLoadOutData _unitLoadOutData;
        // public UnitLoadOutData UnitLoadOutData => _unitLoadOutData;

        public UnitLoadOutData UnitLoadOutData;
        
        // TODO: unit load out data 로 이동
        public Fraction fraction;
        public readonly UnitStatSheet StatSheet = new();

        public Action<Unit> OnSpawnEvent;
        
        private void Awake()
        {
            _draggable = GetComponent<Draggable2D>();

            StatSheet.Health.OnChange += (value) =>
            {
                healthBar.Display(value / StatSheet.MaxHealth.CurrentValue);
            };
        }

        public void MoveToWaitingArea()
        {
            List<ExclusiveDropZone2D> areas = WaitingAreaReferences.Instance.waitingAreas;

            // TODO: ?
            areas.Find((zone) =>
            {
                if (zone.occupant == null)
                {
                    _draggable.MoveToDropZone(zone);
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        // UnitGenerator 에 의해 소환되었을 때
        public void OnSpawn(UnitLoadOutData unitLoadOutData)
        {
            //_unitLoadOutData = unitLoadOutData;
            UnitLoadOutData = unitLoadOutData;
            StatSheet.Init(unitLoadOutData.Stats);
            //TEMP
            GetComponent<SpriteRenderer>().sprite = unitLoadOutData.Unit.Icon;
            
            OnSpawnEvent?.Invoke(this);
        }
    }
}

