using System.Collections.Generic;
using Common.Scripts.TransformChildrenIterator;
using JetBrains.Annotations;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Sell
{
    public class AggressorSideSellManager : MonoBehaviour
    {
        [SerializeField] private GameObject sellsParents;
        private List<AggressorSideSell> _sells;

        protected void Awake()
        {
            _sells = new List<AggressorSideSell>();
            
            foreach (var child in sellsParents.transform.ChildrenForward())
            {
                _sells.Add(child.GetComponent<AggressorSideSell>());
            }
        }
        
        [CanBeNull]
        public AggressorSideSell GetEmptySell()
        {
            return _sells.Find(sell => sell.occupant == null);
        }
    }
}
