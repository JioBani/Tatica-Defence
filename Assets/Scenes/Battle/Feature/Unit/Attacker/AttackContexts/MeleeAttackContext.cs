using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts
{
    /// <summary>
    /// 근접공격
    /// </summary>
    public class MeleeAttackContext : AttackContext
    {
        public MeleeAttackContext(float damage, Attacker attacker,  Victim victim) : base(damage, attacker, victim)
        {
            
        }

        public override void TryAttack()
        {
            Victim.Hit(this);
        }
    }
}