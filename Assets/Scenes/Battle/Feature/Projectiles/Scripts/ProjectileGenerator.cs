using Common.Scripts.ObjectPool;
using Common.Scripts.SceneSingleton;
using UnityEngine;

namespace Scenes.Battle.Feature.Projectiles
{
    public class ProjectileGenerator : SceneSingleton<ProjectileGenerator>
    {
        private ObjectPooler _objectPooler;
        
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileParent;
        
        public Projectile Generate()
        {
            if (_objectPooler == null)
            {
                _objectPooler = ObjectPooler.Instance;
            }

            GameObject newProjectile = _objectPooler.Spawn(
                projectilePrefab,
                projectileParent
            );
            
            var projectile = newProjectile.GetComponent<Projectile>();
            projectile.OnSpawn();

            return projectile;
        }
    }
}