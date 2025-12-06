using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.Rxs;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Rounds.Phases;
using Scenes.Battle.Feature.Unit.Defenders;
using UnityEngine;

namespace Scenes.Battle.Feature.Markets
{
    public class MarketManager : Common.Scripts.SceneSingleton.SceneSingleton<MarketManager>
    {
        [SerializeField] private DefenderManager defenderManager;
        
        public RxValue<int> Gold = new RxValue<int>(0);

        [SerializeField] private List<UnitLoadOutData> appearUnits;
        
        MarketUnitRoller _roller;
        public MarketUnitRoller Roller => _roller;

        private List<MarketDefenderSlot> _defenderSlots;
        public Action<List<UnitLoadOutData>> OnSlotRerolled;
        public Action<OnGoldNotEnoughDto> OnGoldNotEnough;
        
        protected override void OnAwakeSingleton()
        {
            base.OnAwakeSingleton();

            _roller = new MarketUnitRoller(appearUnits);
            
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
            RerollSlots();
        }

        private void RerollSlots()
        {
            List<UnitLoadOutData> units = _roller.PickUnits(4);

            _defenderSlots = units.Select((unit) => new MarketDefenderSlot(unit)).ToList();
            OnSlotRerolled?.Invoke(units);
        }

        public bool BuyDefender(UnitLoadOutData unit)
        {
            if (unit.Unit.Cost > Gold.Value)
            {
                // TODO: UI에 표시
                OnGoldNotEnough?.Invoke(new OnGoldNotEnoughDto());
                return false;
            }
            
            Gold.Value -= unit.Unit.Cost;
            defenderManager.GenerateDefender(unit);
            return true;
        }
    }
}
