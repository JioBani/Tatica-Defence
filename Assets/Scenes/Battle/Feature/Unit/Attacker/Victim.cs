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

        // TODO: AttackContext를 일반화하여 적용하기 전에 Excutable에서만 사용
        public void Hit(float damage)
        {
            Debug.Log(damage);
            unit.StatSheet.Health.CurrentValue -= damage;
        }
    }
}
