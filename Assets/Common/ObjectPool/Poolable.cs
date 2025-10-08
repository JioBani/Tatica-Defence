using UnityEngine;

namespace Common.ObjectPool
{
    public class Poolable : MonoBehaviour
    {
        [SerializeField] private ObjectPooler pooler;
        
        void DeSpawn()
        {
            pooler.DeSpawn(this);
        }
    }    
}

