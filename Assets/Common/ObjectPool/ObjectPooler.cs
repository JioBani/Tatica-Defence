using System.Collections.Generic;
using UnityEngine;

namespace Common.ObjectPool
{
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        [SerializeField] private List<GameObject> poolableObjects;

        public GameObject Spawn(Vector2? position = null)
        {
            GameObject target = poolableObjects.Find(poolable => !poolable.activeSelf);

            if (target == null)
            {
                target = Instantiate(prefab, transform, true);

                target.AddComponent<Poolable>();

                poolableObjects.Add(target);
            }

            if (position.HasValue)
            {
                target.transform.position = position.Value;
            }
            
            target.SetActive(true);
            return target;
        }

        public void DeSpawn(Poolable poolable)
        {
            if (poolableObjects.Contains(poolable.gameObject))
            {
                Debug.LogError("Poolable 이 해당 부모의 자식이 아닙니다.");
                return;
            }

            poolable.gameObject.SetActive(false);
            poolable.transform.SetParent(transform);
            poolable.transform.position = transform.position;
        }
    }   
}
