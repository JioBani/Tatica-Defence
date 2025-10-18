using Common.Scripts.Enums;
using Scenes.Battle.Feature.Rounds.Unit.HUDs.HealthBar;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scenes.Battle.Feature.Units.Attackable
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
