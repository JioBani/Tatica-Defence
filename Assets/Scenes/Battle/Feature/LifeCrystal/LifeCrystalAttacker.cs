using UnityEngine;

namespace Scenes.Battle.Feature.LifeCrystals
{
    public class LifeCrystalAttacker : MonoBehaviour
    {
        private LifeCrystalManager _lifeCrystalManager;
        
        private void Awake()
        {
            _lifeCrystalManager = GameObject.Find("LifeCrystalManager").GetComponent<LifeCrystalManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("LifeCrystalContactZone"))
            {
                _lifeCrystalManager.ChangeLifePoint(-10);
            }
        }
    }
}
