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

        public Unit GenerateAggressorSample(UnitLoadOutData data)
        {
            return Generate(data, aggressorSamplePrefab);
        }
    }
}
