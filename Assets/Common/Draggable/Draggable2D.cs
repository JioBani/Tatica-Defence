using UnityEngine;
using UnityEngine.InputSystem;

namespace Common.Draggable
{
    public class Draggable2D : MonoBehaviour
    {
        bool isDragging = false;

        private Camera camera;
        private Vector2 startPostion;
    
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
        
            FindDropZone();
        }

        void Update()
        {
            if (isDragging)
            {
                transform.position = camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void FindDropZone()
        {
            Collider2D hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("DropZone"));
            
            if (hit == null)
            {
                transform.position = startPostion;
                return;
            }
        
            IDropZone2D zone = null;
            zone = hit.GetComponent<IDropZone2D>();

            if (zone == null)
            {
                transform.position = startPostion;
                return; 
            }

            if (!zone.CanAccept(this))
            {
                transform.position = startPostion;
                return; 
            }

            transform.position = new Vector3(
                hit.gameObject.transform.position.x, 
                hit.gameObject.transform.position.y,
                transform.position.z
            );
        
            zone.OnDrop(this);
            
            startPostion = transform.position;
        }
    }
}


