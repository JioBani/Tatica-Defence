using Scenes.Battle.Feature.Units.Attackables;
using Scenes.Battle.Feature.Units.Attackers;

namespace Scenes.Battle.Feature.Unit.Skills.Executable
{
    public class RangeAttackExecutor : IExecutable
    {
        private readonly Attacker _attacker;
        private readonly Victim _victim;

        public RangeAttackExecutor(Attacker attacker, Victim victim)
        {
            _attacker = attacker;
            _victim = victim;
        }
        
        public void Execute()
        {
            _victim.Hit( _attacker.Unit.StatSheet.PhysicalAttack.CurrentValue);
        }
    }
}