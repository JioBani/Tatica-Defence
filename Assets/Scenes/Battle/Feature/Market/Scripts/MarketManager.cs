using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.Rxs;
using Common.Scripts.StateBase;
using JetBrains.Annotations;
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
        [SerializeField] private List<UnitLoadOutData> appearUnits;

        [SerializeField] private int levelUpGold = 5;
        
        public readonly RxValue<int> Gold = new RxValue<int>(10);
        public readonly RxValue<int> DefenderPlacementLimit = new RxValue<int>(0);
        public readonly RxValue<int> Level = new RxValue<int>(1);

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

        private bool BuySomething(int gold, string notEnoughGoldMessage = null)
        {
            if (gold > Gold.Value)
            {
                // TODO: UI에 표시
                OnGoldNotEnough?.Invoke(new OnGoldNotEnoughDto());
                if (notEnoughGoldMessage != null)
                {
                    AlertManager.Instance.Alert(notEnoughGoldMessage);
                }
                return false;
            }
            else
            {
                Gold.Value -= gold;
                return true;
            }
        }

        public bool BuyDefender(UnitLoadOutData unit)
        {
            if (BuySomething(unit.Unit.Cost, "골드가 부족합니다."))
            {
                defenderManager.GenerateDefender(unit);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsDefenderLimitExceeded()
        {
            return  defenderManager.GetPlacementCount(Placement.BattleArea) >= DefenderPlacementLimit.Value;
        }

        public bool LevelUp()
        {
            if (BuySomething(levelUpGold, "골드가 부족합니다."))
            {
                Level.Value += 1;
                DefenderPlacementLimit.Value += 1; // 레벨업시 수호자 배치 상한 상승
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
