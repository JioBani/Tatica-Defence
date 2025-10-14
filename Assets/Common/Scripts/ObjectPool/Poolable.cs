using UnityEngine;

namespace Common.Scripts.ObjectPool
{
    public class Poolable : MonoBehaviour
    {
        public string poolId { get; private set; }

        public void DeSpawn()
        {
            ObjectPooler.Instance.DeSpawn(this);
        }

        public void SetPoolId(string id)
        {
            poolId = id;
        }
    }    
}

