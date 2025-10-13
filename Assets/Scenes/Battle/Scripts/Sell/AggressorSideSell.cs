using UnityEngine;

namespace Scenes.Battle.Scripts.Sell
{
    public class AggressorSideSell : MonoBehaviour
    {
       public Unit.Unit occupant { get; private set; }

        public void SetUnit(Unit.Unit unit)
        {
            occupant = unit;
        }
    }
}
