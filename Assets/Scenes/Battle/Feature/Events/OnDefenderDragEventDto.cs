using Common.Scripts.Draggable;
using Common.Scripts.GlobalEventBus;
using Scenes.Battle.Feature.Unit.Defenders;

namespace Scenes.Battle.Feature.Events
{
    public struct OnDefenderDragEventDto : IGameEvent
    {
        public OnDefenderDragEventDto(Defender defender, DragState state)
        {
            this.defender = defender;
            this.state = state;
        }
        
        public Defender defender { get;}
        public DragState state { get;}
    }
}

