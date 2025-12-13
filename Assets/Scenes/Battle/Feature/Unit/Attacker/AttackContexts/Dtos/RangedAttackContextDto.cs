using Scenes.Battle.Feature.Projectiles;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos
{
    public class RangedAttackContextDto : AttackContextDto
    {
        public Projectile Projectile { get; private set; }

        public RangedAttackContextDto(
            float damage, 
            Attacker attacker, 
            Victim victim, 
            Projectile projectile
        ) : base(damage, attacker, victim)
        {
            Projectile = projectile;
        }
    }
}