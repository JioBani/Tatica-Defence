using Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Units.Attackables
{
    public abstract class AttackContext
    {
        public float damage;
        protected Victim Victim;
        protected Attacker Attacker;

        public AttackContext(AttackContextDto dto)
        {
            this.damage = dto.Damage;
            Attacker = dto.Attacker;
            Victim = dto.Victim;
        }

        public abstract void TryAttack();
    }
}