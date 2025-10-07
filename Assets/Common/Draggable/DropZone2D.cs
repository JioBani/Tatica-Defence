using UnityEngine;

namespace Common.Draggable
{
    public interface IDropZone2D
    {
        bool CanAccept(Draggable2D item);
        void OnDrop(Draggable2D item);
    }

    [RequireComponent(typeof(Collider2D))]
    public class DropZone2D : MonoBehaviour, IDropZone2D
    {
        [Tooltip("이 존이 받는 태그. 비우면 아무나 수용.")]
        public string requiredTag = "";
        public bool snapToCenter = true;

        public bool CanAccept(Draggable2D item)
        {
            if (requiredTag == "")
            {
                return true;
            }

            return item.CompareTag(requiredTag);
        }

        public void OnDrop(Draggable2D item)
        {
        
        }
    }
}

