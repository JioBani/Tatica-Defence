using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.ObjectPool;
using UnityEngine;

namespace Scenes.Battle.Feature.Units
{
    public class UnitGenerator : MonoBehaviour
    {
        private ObjectPooler _objectPooler;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private GameObject aggressorPrefab;
        [SerializeField] private GameObject aggressorSamplePrefab;
        
        [SerializeField] private Transform enemyField;
        
        private Unit Generate(UnitLoadOutData data, GameObject prefab)
        {
            if (_objectPooler == null)
            {
                _objectPooler = ObjectPooler.Instance;
            }

            GameObject newUnit = _objectPooler.Spawn(
                prefab,
                enemyField
            );
            
            var unitComponent = newUnit.GetComponent<Unit>();
            
            unitComponent.OnSpawn(data);

            return unitComponent;
        }

        public Unit GenerateDefender(UnitLoadOutData data)
        {
            return Generate(data, unitPrefab);
        }

        
        public Unit GenerateAggressor(UnitLoadOutData data)
        {
            return Generate(data, aggressorPrefab);
        }

        public Unit GenerateAggressorSample(UnitLoadOutData data)
        {
            return Generate(data, aggressorSamplePrefab);
        }
    }
}
