using UnityEngine;

namespace Common.Scripts.Draggable
{
    public class Draggable2D : MonoBehaviour
    {
        private bool _isDragging = false;

        private Camera _camera;
        private Vector2 _startPosition;
        public DropZone2D thisDropZone;
    
        void Awake()
        {
            _camera = Camera.main;
        }

        public void BeginDrag()
        {
            _startPosition = transform.position;
            _isDragging = true;
        }

        public void EndDrag()
        {
            _isDragging = false;
        
            TryDrop();
        }

        void Update()
        {
            if (_isDragging)
            {
                transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void TryDrop()
        {
            Collider2D hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("DropZone"));
            
            if (hit == null)
            {
                transform.position = _startPosition;
                return;
            }
        
            DropZone2D zone = null;
            zone = hit.GetComponent<DropZone2D>();

            if (zone == null)
            {
                transform.position = _startPosition;
                return; 
            }

            if (!zone.CanAccept(this,thisDropZone))
            {
                transform.position = _startPosition;
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
            
            _startPosition = transform.position;
            
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


