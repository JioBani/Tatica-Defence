using System;
using Common.Data.Units.UnitStatsByLevel;
using Unity.VisualScripting;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.Attackable
{
    public class Attacker : MonoBehaviour
    {
        [SerializeField] private float range = 3;
        [SerializeField] private Unit _unit;
        public Unit Unit => _unit;
        
        private CircleCollider2D _circleCollider2D;
        private Victim _victim;

        private void Awake()
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _unit.onSpawnEvent += SetRange;
        }

        private void SetRange()
        {
            _circleCollider2D.radius = Unit.UnitLoadOutData.Stats.GetStat(UnitStatKind.AttackRange, 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_victim == null && other.CompareTag("Victim"))
            {
                Victim newVictim = other.GetComponent<Victim>();

                if (newVictim.Unit.fraction != Unit.fraction)
                {
                    _victim = newVictim;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_victim != null && other.CompareTag("Victim") && _victim == other.GetComponent<Victim>())
            {
                _victim = null;
            }
        }
    }
}
