using Common.Scripts.SceneSingleton;
using Scenes.Battle.Feature.Projectiles;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts
{
    public class AttackContextFactory : SceneSingleton<AttackContextFactory>
    {
        [SerializeField] ProjectileGenerator generator;
        
        public MeleeAttackContext GenerateMelee(float damage, Attacker attacker, Victim victim)
        {
            return new MeleeAttackContext(damage, attacker, victim);
        }

        public RangedAttackContext GenerateRanged(float damage, Attacker attacker, Victim victim)
        {
            return new RangedAttackContext(damage, attacker, victim, generator.Generate());
        }
    }
}