using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.Rxs;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Rounds.Phases;
using Scenes.Battle.Feature.Ui;
using Scenes.Battle.Feature.Unit.Defenders;
using UnityEngine;

namespace Scenes.Battle.Feature.Markets
{
    public class MarketManager : Common.Scripts.SceneSingleton.SceneSingleton<MarketManager>
    {
        [SerializeField] private DefenderManager defenderManager;
        
        public readonly RxValue<int> Gold = new RxValue<int>(10);
        public readonly RxValue<int> DefenderPlacementLimit = new RxValue<int>(0);

        [SerializeField] private List<UnitLoadOutData> appearUnits;
        
        MarketUnitRoller _roller;
        public MarketUnitRoller Roller => _roller;

        public Action<List<UnitLoadOutData>> OnSlotRerolled;
        public Action<OnGoldNotEnoughDto> OnGoldNotEnough;
        
        protected override void OnAwakeSingleton()
        {
            base.OnAwakeSingleton();

            _roller = new MarketUnitRoller(appearUnits);
            
            RoundManager
                .Instance
                .AddOnConfigureStatesEvent(AddOnRoundStartEvent);

            // TOOD: RoundManager 에 게임 시작 상태를 만들고 그곳에 콜백 등록
            DefenderPlacementLimit.Value = 3;
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

            OnSlotRerolled?.Invoke(units);
        }

        public bool BuyDefender(UnitLoadOutData unit)
        {
            if (unit.Unit.Cost > Gold.Value)
            {
                // TODO: UI에 표시
                OnGoldNotEnough?.Invoke(new OnGoldNotEnoughDto());
                AlertManager.Instance.Alert("골드 부족");
                return false;
            }
            
            Gold.Value -= unit.Unit.Cost;
            defenderManager.GenerateDefender(unit);
            return true;
        }

        public bool IsDefenderLimitExceeded()
        {
            return  defenderManager.CalculateDefenderCount(Placement.BattleArea) >= DefenderPlacementLimit.Value;
        }
    }
}
