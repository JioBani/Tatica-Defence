using Common.Scripts.Enums;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.Attackable
{
    public class Victim : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        public Unit Unit => _unit;

        public void Hit(AttackContext context)
        {
            
        }
    }
}
