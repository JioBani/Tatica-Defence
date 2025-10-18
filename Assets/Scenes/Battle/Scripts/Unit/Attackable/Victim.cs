using Common.Scripts.Enums;
using Scenes.Battle.Scripts.Unit.HUDs.HealthBar;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Battle.Scripts.Unit.Attackable
{
    public class Victim : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        public Unit Unit => unit;

        public void Hit(AttackContext context)
        {
            unit.StatSheet.Health.CurrentValue -= context.damage;
        }
    }
}
