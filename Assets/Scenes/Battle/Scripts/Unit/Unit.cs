using System;
using System.Collections.Generic;
using Common.Draggable;
using Scenes.Battle.Scripts.WaitingArea;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit
{
    // TODO: 기능이 많아지는 경우 분리
    public class Unit : MonoBehaviour
    {
        private Draggable2D _draggable;

        private void Awake()
        {
            _draggable = GetComponent<Draggable2D>();
        }

        private void OnEnable()
        {
            MoveToWaitingArea();
        }

        private void MoveToWaitingArea()
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
    }
}

