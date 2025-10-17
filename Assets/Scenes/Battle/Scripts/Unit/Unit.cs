using System;
using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.Draggable;
using Common.Scripts.Enums;
using Scenes.Battle.Scripts.WaitingArea;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit
{
    // TODO: 기능이 많아지는 경우 분리
    public class Unit : MonoBehaviour
    {
        private Draggable2D _draggable;
        // private UnitLoadOutData _unitLoadOutData;
        // public UnitLoadOutData UnitLoadOutData => _unitLoadOutData;

        public UnitLoadOutData UnitLoadOutData;

        // TODO: unit load out data 로 이동
        public Fraction fraction;

        public Action<Unit> onSpawnEvent;
        
        private void Awake()
        {
            _draggable = GetComponent<Draggable2D>();
        }

        public void MoveToWaitingArea()
        {
            List<ExclusiveDropZone2D> areas = WaitingAreaReferences.Instance.waitingAreas;

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
            //TEMP
            GetComponent<SpriteRenderer>().sprite = unitLoadOutData.Unit.Icon;
            
            onSpawnEvent?.Invoke(this);
        }
    }
}

