using Common.Data.Units.UnitLoadOuts;

namespace Scenes.Battle.Feature.Markets
{
    public class MarketDefenderSlot
    {
        public bool IsSold { get; private set; }
        public UnitLoadOutData UnitLoadOutData { get; private set; }

        public MarketDefenderSlot(UnitLoadOutData unitLoadOutData)
        {
            UnitLoadOutData = unitLoadOutData;
            IsSold = false;
        }
    }
}