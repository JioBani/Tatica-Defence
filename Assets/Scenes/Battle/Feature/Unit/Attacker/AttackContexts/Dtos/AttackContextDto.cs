using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos
{
    // TODO: 확장이 필요한 경우 확장
    public class AttackContextDto
    {
        public float Damage { get; set; }
        public Attacker Attacker { get; set; }
        public Victim Victim { get; set; }

        public AttackContextDto(float damage, Attacker attacker, Victim victim)
        {
            Damage = damage;
            Attacker = attacker;
            Victim = victim;
        }
    }
}