namespace Scenes.Battle.Feature.Rounds.Phases
{
    public class ReadyPhase : Phase
    {
        
        public ReadyPhase() : base(PhaseType.Ready)
        {
            
        }

        public override void OnEnter()
        {
            Exit();
        }

        public override void OnRun()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override PhaseType GetNextPhase()
        {
            return PhaseType.Combat;
        }
    }
}