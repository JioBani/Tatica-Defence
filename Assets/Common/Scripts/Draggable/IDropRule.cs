namespace Common.Scripts.Draggable
{
    /// <summary>
    /// Drop Zone 에 Drop 할 수 있는지에 대한 규칙
    /// </summary>
    public interface IDropRule
    {
        bool CanAccept(Draggable2D draggable, DropZone2D before, DropZone2D after);

        void OnDropped(Draggable2D draggable, DropZone2D before, DropZone2D after);
        
        void OnDragOut(Draggable2D item, DropZone2D zone);
    }
}

