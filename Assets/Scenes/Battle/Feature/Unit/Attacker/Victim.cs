using UnityEngine;

namespace Scenes.Battle.Feature.Units.Attackables
{
    public class Victim : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        public Unit Unit => unit;

        public void Hit(AttackContext context)
        {
            Debug.Log(context.damage);
            unit.StatSheet.Health.CurrentValue -= context.damage;
        }
    }
}
