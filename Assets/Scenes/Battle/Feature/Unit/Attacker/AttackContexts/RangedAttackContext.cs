using Scenes.Battle.Feature.Projectiles;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts
{
    public class RangedAttackContext : AttackContext
    {
        private readonly Projectile _projectile;
        
        public RangedAttackContext(float damage, Attacker attacker, Victim victim, Projectile projectile) 
            : base(damage,attacker, victim)
        {
            _projectile = projectile;
            projectile.OnHit += OnHit;
        }

        public override void TryAttack()
        {
            _projectile.Shot(Attacker, Victim.transform);
        }

        private void OnHit()
        {
            Debug.Log("RangedAttackContext.Hit");
            Victim.Hit(this);
            _projectile.OnHit -= OnHit;
        }
    }
}