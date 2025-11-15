using System;
using Common.Scripts.Rxs;
using Common.Scripts.SceneSingleton;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Rounds.Phases;
using UnityEngine;

namespace Scenes.Battle.Feature.Markets
{
    public class MarketManager : SceneSingleton<MarketManager>
    {
        public RxValue<int> Gold = new RxValue<int>(0);

        protected override void OnAwakeSingleton()
        {
            base.OnAwakeSingleton();
            
            RoundManager
                .Instance
                .AddOnConfigureStatesEvent(AddOnRoundStartEvent);
        }

        private void AddOnRoundStartEvent(object _)
        {
            RoundManager
                .Instance
                .GetStateBase(PhaseType.Maintenance)
                .Event
                .Add(
                    StateBaseEventType.Enter,
                    (_,_) => OnRoundStart()
                );
        }

        private void OnRoundStart()
        {
            Gold.Value += 5;
        }
    }
    
}
