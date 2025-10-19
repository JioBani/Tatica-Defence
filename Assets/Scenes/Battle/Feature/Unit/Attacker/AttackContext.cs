using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Units.Attackables
{
    public abstract class AttackContext
    {
        public float damage;
        protected Victim Victim;
        protected Attacker Attacker;

        public AttackContext(float damage, Attacker attacker, Victim victim)
        {
            this.damage = damage;
            Attacker = attacker;
            Victim = victim;
        }

        public abstract void TryAttack();
    }
}