using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Unit.Defenders;
using Scenes.Battle.Feature.Units;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class CombatPhase : StateBase<PhaseType>
    {
        private UnitGenerator _unitGenerator;
        private readonly RoundAggressorManager _roundAggressorManager;
        private readonly DefenderManager _defenderManager;

        public CombatPhase(
            RoundAggressorManager roundAggressorManager,
            DefenderManager defenderManager,
            StateBaseController<PhaseType> controller
        ) : base(
            PhaseType.Combat,
            controller
        )
        {
            _roundAggressorManager = roundAggressorManager;
            _defenderManager = defenderManager;
        }

        public override void OnEnter()
        {
            Debug.Log("Combat Phase OnEnter");
        }

        public override void OnRun()
        {
            if (_roundAggressorManager.IsAllAggressorsCompleted())
            {
                Exit(PhaseType.Maintenance);
            }

            if (_defenderManager.IsAllDefenderDowned())
            {
                Exit(PhaseType.GameOver);
            }
        }

        public override void OnExit()
        {
            Debug.Log("Combat Phase OnExit");
        }

        public override void Dispose()
        {
            
        }
    }
}