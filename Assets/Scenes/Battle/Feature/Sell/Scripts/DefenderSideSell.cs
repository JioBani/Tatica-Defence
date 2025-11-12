using System;
using System.Collections.Generic;
using Common.Scripts.Draggable;
using Common.Scripts.InspectorDescriptionAttributes;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Rounds.Phases;
using UnityEngine;

namespace Scenes.Battle.Feature.Sells
{
    
    [InspectorDescription("정비 페이즈에서만 수호자 배치 가능하도록 룰 추가한 확장 Sell",InspectorMessageType.Info)]
    
    public class DefenderSideSell : ExclusiveDropZone2D, IDropRule
    {
        void Awake()
        {
            AddRule(this);
        }

        public bool CanAccept(Draggable2D draggable, DropZone2D before, DropZone2D after)
        {
            if (RoundManager.Instance.GetCurrentState().StateType == PhaseType.Maintenance)
            {
                return true;                
            }
            else
            {
                Debug.Log("수호자는 준비 상태에서만 배치 할 수 있습니다.");
                return false;
            }
        }

        public void OnDropped(Draggable2D draggable, DropZone2D before, DropZone2D after)
        {
        }

        public void OnDragOut(Draggable2D item, DropZone2D zone)
        {
        }
    }
}
