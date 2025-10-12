using System;
using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.Draggable;
using Scenes.Battle.Scripts.WaitingArea;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit
{
    // TODO: 기능이 많아지는 경우 분리
    public class Unit : MonoBehaviour
    {
        private Draggable2D _draggable;
        private UnitLoadOutData _unitLoadOutData;

        private void Awake()
        {
            _draggable = GetComponent<Draggable2D>();
        }

        private void OnEnable()
        {
            //MoveToWaitingArea();
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

        public void OnSpawn(UnitLoadOutData unitLoadOutData)
        {
            _unitLoadOutData = unitLoadOutData;
            
            //TEMP
            GetComponent<SpriteRenderer>().sprite = unitLoadOutData.Unit.Icon;
        }
    }
}

