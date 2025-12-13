using Scenes.Battle.Feature.Unit.Attackers.AttackContexts.Dtos;
using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Attackers.AttackContexts
{
    /// <summary>
    /// 근접공격
    /// </summary>
    public class MeleeAttackContext : AttackContext
    {
        public MeleeAttackContext(AttackContextDto dto) : base(dto)
        {
            
        }

        public override void TryAttack()
        {
            Victim.Hit(this);
        }
    }
}