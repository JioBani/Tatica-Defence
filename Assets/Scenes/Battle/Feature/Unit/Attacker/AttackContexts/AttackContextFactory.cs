using Common.Scripts.SceneSingleton;
using Scenes.Battle.Feature.Projectiles;
using Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;
using UnityEngine;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts
{
    public class AttackContextFactory : SceneSingleton<AttackContextFactory>
    {
        [SerializeField] ProjectileGenerator generator;
        
        public MeleeAttackContext GenerateMelee(AttackContextDto dto)
        {
            return new MeleeAttackContext(dto);
        }

        public RangedAttackContext GenerateRanged(AttackContextDto dto)
        {
            return new RangedAttackContext(new RangedAttackContextDto(
                dto.Damage,
                dto.Attacker,
                dto.Victim,
                generator.Generate()
            ));
        }
    }
}