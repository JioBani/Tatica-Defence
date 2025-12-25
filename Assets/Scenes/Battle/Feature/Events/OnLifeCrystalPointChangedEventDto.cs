using Common.Scripts.GlobalEventBus;

namespace Scenes.Battle.Feature.Events
{
    public struct OnLifeCrystalPointChangedEventDto : IGameEvent
    {
        public readonly int LifePoint;

        public OnLifeCrystalPointChangedEventDto(int lifePoint)
        {
            LifePoint = lifePoint;
        }
    }
}