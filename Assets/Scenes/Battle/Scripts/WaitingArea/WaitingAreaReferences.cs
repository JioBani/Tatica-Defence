using System.Collections.Generic;
using Common.Draggable;
using Common.SceneSingleton;
using Common.TransformChildrenIterator;
using UnityEngine;

namespace Scenes.Battle.Scripts.WaitingArea
{
    public class WaitingAreaReferences : SceneSingleton<WaitingAreaReferences>
    {
        [SerializeField] private GameObject waitingAreaParent;
        public List<ExclusiveDropZone2D> waitingAreas;

        protected override void Awake()
        {
            base.Awake();
            
            waitingAreas =  new List<ExclusiveDropZone2D>();

            foreach (var child in waitingAreaParent.transform.ChildrenForward())
            {
                waitingAreas.Add(child.GetComponent<ExclusiveDropZone2D>());
            }
        }
    }
}

