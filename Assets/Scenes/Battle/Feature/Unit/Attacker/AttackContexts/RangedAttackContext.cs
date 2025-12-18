using Scenes.Battle.Feature.Projectiles;
using Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts
{
    public class RangedAttackContext : AttackContext
    {
        private readonly Projectile _projectile;
        
        public RangedAttackContext(RangedAttackContextDto dto) 
            : base(dto)
        {
            Debug.Log(dto.Projectile);
            _projectile = dto.Projectile;
            _projectile.OnHit += OnHit;
        }

        public override void TryAttack()
        {
            _projectile.Shot(Attacker.transform, Victim.transform);
        }

        private void OnHit()
        {
            Debug.Log("RangedAttackContext.Hit");
            Victim.Hit(this);
            _projectile.OnHit -= OnHit;
        }
    }
}