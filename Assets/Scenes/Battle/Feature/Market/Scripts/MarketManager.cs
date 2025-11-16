using System;
using Common.Data.Configs;
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
        [SerializeField] private EconomyConfig economyConfig;

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
            Gold.Value += GetRoundStartIncome();
        }
        
        private int GetRoundStartIncome()
        {
            int income = economyConfig.baseGoldPerRound;

            // 이자
            int steps = Mathf.Min(Gold.Value / economyConfig.goldPerInterestStep, economyConfig.maxInterest);
            income += steps * economyConfig.interestPerStep;

            // 연승 보너스
            // if (winStreak > 0 && winStreak < winStreakBonusByCount.Length)
            //     income += winStreakBonusByCount[winStreak];
            //
            // // 연패 보너스
            // if (loseStreak > 0 && loseStreak < loseStreakBonusByCount.Length)
            //     income += loseStreakBonusByCount[loseStreak];

            return income;
        }
    }
    
}
