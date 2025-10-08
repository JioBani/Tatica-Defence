using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Draggable
{
    [RequireComponent(typeof(Collider2D))]
    public class DropZone2D : MonoBehaviour
    {
        [Tooltip("이 존이 받는 태그. 비우면 아무나 수용.")]
        public string requiredTag = "";
        public bool snapToCenter = true;

        private readonly List<IDropRule> rules = new();

        public bool CanAccept(Draggable2D item, DropZone2D before)
        {
            if (requiredTag != "" && !item.CompareTag(requiredTag)) return false;
            return !rules.Exists(rule => !rule.CanAccept(item, before,this));
        }

        public virtual void OnDrop(Draggable2D draggable, DropZone2D before)
        {
            rules.ForEach(rule => rule.OnDropped(draggable, before, this));
        }

        public virtual void OnDragOut(Draggable2D item)
        {
            rules.ForEach(rule => rule.OnDragOut(item, this));
        }

        public void AddRule(IDropRule rule) => rules.Add(rule);
    }
}
