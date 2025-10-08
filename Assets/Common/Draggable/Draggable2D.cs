using UnityEngine;
using UnityEngine.InputSystem;

namespace Common.Draggable
{
    public class Draggable2D : MonoBehaviour
    {
        bool isDragging = false;

        private Camera camera;
        private Vector2 startPostion;
        public DropZone2D thisDropZone;
    
        void Awake()
        {
            camera = Camera.main;
        }

        public void BeginDrag()
        {
            startPostion = transform.position;
            isDragging = true;
        }

        public void EndDrag()
        {
            isDragging = false;
        
            TryDrop();
        }

        void Update()
        {
            if (isDragging)
            {
                transform.position = camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void TryDrop()
        {
            Collider2D hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("DropZone"));
            
            if (hit == null)
            {
                transform.position = startPostion;
                return;
            }
        
            DropZone2D zone = null;
            zone = hit.GetComponent<DropZone2D>();

            if (zone == null)
            {
                transform.position = startPostion;
                return; 
            }

            if (!zone.CanAccept(this,thisDropZone))
            {
                transform.position = startPostion;
                return; 
            }
            
            MoveToDropZone(zone);
        }

        public void MoveToDropZone(DropZone2D newZone)
        {
            transform.position = new Vector3(
                newZone.transform.position.x, 
                newZone.transform.position.y,
                transform.position.z
            );
            
            startPostion = transform.position;
            
            if (thisDropZone != null)
            {
                thisDropZone.OnDragOut(this);
            }
            
            DropZone2D oldZone = thisDropZone;
            
            thisDropZone = newZone;
            
            newZone.OnDrop(this, oldZone);
        }
    }
}


