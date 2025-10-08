using System;
using System.Collections.Generic;
using Common.Draggable;
using UnityEngine;

namespace Scenes.Battle.Scripts.WaitingArea
{
    [RequireComponent(typeof(DropZone2D))]
    public class WaitingArea : MonoBehaviour, IDropRule
    {
        [SerializeField] private Draggable2D draggable;

        private GameObject occupantUnit;

        private DropZone2D dropZone;
        
        private void Awake()
        {
            dropZone = GetComponent<DropZone2D>();
            
            dropZone.AddRule(this);
        }

        public bool CanAccept(Draggable2D item, DropZone2D zone)
        {
            return occupantUnit == null;
        }

        public void OnDropped(Draggable2D item, DropZone2D zone)
        {
            occupantUnit = item.gameObject; //TODO: 이후 Unit 으로 변경
        }

        public void OnDragOut(Draggable2D item, DropZone2D zone)
        {
            occupantUnit = null;
        }
    }
}


