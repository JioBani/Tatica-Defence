using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.ObjectPool;
using Scenes.Battle.Scripts.ObjectPool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Battle.Scripts.Unit
{
    public class UnitGenerator : MonoBehaviour
    {
        private ObjectPooler _objectPooler;
        [SerializeField] private GameObject unitPrefab;
        
        [SerializeField] private Transform enemyField;
        
        public Unit Generate(UnitLoadOutData data)
        {
            if (_objectPooler == null)
            {
                _objectPooler = ObjectPooler.Instance;
            }

            GameObject newUnit = _objectPooler.Spawn(
                unitPrefab,
                enemyField
            );
            
            var unitComponent = newUnit.GetComponent<Unit>();
            
            
            unitComponent.OnSpawn(data);

            return unitComponent;
        }
    }
}
