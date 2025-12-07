using Common.Scripts.GlobalEventBus;
using Scenes.Battle.Feature.Unit.Defenders;

namespace Scenes.Battle.Feature.Events
{
    public struct OnDefenderPlacementChangedEventDto : IGameEvent
    {
        public OnDefenderPlacementChangedEventDto(Defender defender, Placement placement)
        {
            this.defender = defender;
            this.placement = placement;
        }

        public Defender defender { get; private set; }
        public Placement placement { get; private set; }
    }
}
