using Scenes.Battle.Feature.Projectiles;
using Scenes.Battle.Feature.Unit.Skills.Executable;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Skills.Castables
{
    public class AttackCast : Castable
    {
        private readonly Attacker _attacker;

        public AttackCast(Attacker attacker)
        {
            _attacker = attacker;
        }
        
        public override bool CanCast()
        {
            return true;
        }

        public override IExecutable Cast()
        {
            RangeAttackExecutor executor = new RangeAttackExecutor(_attacker, _attacker.Victim);

            var projectile = ProjectileGenerator.Instance.Generate();

            projectile.OnHit += executor.Execute;
            
            projectile.Shot(_attacker.transform, _attacker.Victim.transform);
            
            return new RangeAttackExecutor(_attacker, _attacker.Victim);
        }
    }
}