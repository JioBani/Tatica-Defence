using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.ObjectPool;
using UnityEngine;

namespace Scenes.Battle.Feature.Units
{
    public class UnitGenerator : MonoBehaviour
    {
        private ObjectPooler _objectPooler;
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private GameObject defenderPrefab;
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

        // TODO: 어떤 종류의 Unit 을 생성할것인지는 Defender Manager 등에 위임하고, Unit 만 생성하도록 수정
        public Unit GenerateDefender(UnitLoadOutData data)
        {
            return Generate(data, defenderPrefab);
        }
        
        public Unit GenerateAggressor(UnitLoadOutData data)
        {
            return Generate(data, aggressorPrefab);
        }

        public Unit GenerateAggressorSample(UnitLoadOutData data)
        {
            return Generate(data, aggressorSamplePrefab);
        }

        public void RemoveUnit(Unit unit)
        {
            _objectPooler.DeSpawn(unit.GetComponent<Poolable>());
        }
    }
}
