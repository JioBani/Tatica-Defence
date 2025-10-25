using UnityEngine;

namespace Scenes.Battle.Feature.Sells
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
