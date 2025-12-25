using Scenes.Battle.Feature.Aggressors;
using UnityEngine;

namespace Scenes.Battle.Feature.LifeCrystals
{
    public class LifeCrystalAttacker : MonoBehaviour
    {
        private LifeCrystalManager _lifeCrystalManager;
        private Aggressor _aggressors;
        
        private void Awake()
        {
            _lifeCrystalManager = GameObject.Find("LifeCrystalManager").GetComponent<LifeCrystalManager>();
            _aggressors = GetComponent<Aggressor>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("LifeCrystalContactZone"))
            {
                _lifeCrystalManager.ChangeLifePoint(-10);
                _aggressors.OnEnterLifeCrystalContactZone();
            }
        }
    }
}
