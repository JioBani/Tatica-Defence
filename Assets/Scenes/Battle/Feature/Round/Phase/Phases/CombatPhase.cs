using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Units;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class CombatPhase : StateBase<PhaseType>
    {
        private UnitGenerator _unitGenerator;
        private RoundAggressorManager _roundAggressorManager;

        public CombatPhase(
            RoundAggressorManager roundAggressorManager,
            StateBaseController<PhaseType> controller
        ) : base(
            PhaseType.Combat,
            controller
        )
        {
            _roundAggressorManager = roundAggressorManager;
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