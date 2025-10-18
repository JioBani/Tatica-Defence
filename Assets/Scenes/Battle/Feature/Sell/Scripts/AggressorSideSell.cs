using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Sell
{
    public class AggressorSideSell : MonoBehaviour
    {
       public Feature.Units.Unit occupant { get; private set; }

        public void SetUnit(Feature.Units.Unit unit)
        {
            occupant = unit;
        }
    }
}
