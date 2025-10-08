using UnityEngine;

namespace Common.ObjectPool
{
    public class Poolable : MonoBehaviour
    {
        [SerializeField] private ObjectPooler pooler;
        public string poolId { get; private set; }

        void DeSpawn()
        {
            pooler.DeSpawn(this);
        }

        public void SetPoolId(string id)
        {
            poolId = id;
        }
    }    
}

