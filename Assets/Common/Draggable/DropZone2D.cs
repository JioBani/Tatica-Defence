using System.Collections.Generic;
using UnityEngine;

namespace Common.Draggable
{
    [RequireComponent(typeof(Collider2D))]
    public class DropZone2D : MonoBehaviour
    {
        [Tooltip("이 존이 받는 태그. 비우면 아무나 수용.")]
        public string requiredTag = "";
        public bool snapToCenter = true;

        private List<IDropRule> rules = new List<IDropRule>();

        public bool CanAccept(Draggable2D item)
        {
            if (requiredTag != "" && !item.CompareTag(requiredTag))
            {
                return false;
            }

            return !rules.Exists(rule => !rule.CanAccept(item, this));
        }

        public void OnDrop(Draggable2D item)
        {
            rules.ForEach(rule => rule.OnDropped(item, this));
        }

        public void OnDragOut(Draggable2D item)
        {
            rules.ForEach(rule => rule.OnDragOut(item, this));
        }

        public void AddRule(IDropRule rule)
        {
            rules.Add(rule);
        }
    }
}

