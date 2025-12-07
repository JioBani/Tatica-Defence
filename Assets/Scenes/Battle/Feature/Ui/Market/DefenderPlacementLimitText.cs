using System;
using Common.Scripts.StateBase;
using Scenes.Battle.Feature.Markets;
using Scenes.Battle.Feature.Rounds;
using Scenes.Battle.Feature.Rounds.Phases;
using Scenes.Battle.Feature.Unit.Defenders;
using TMPro;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui.Markets
{
    public class DefenderPlacementLimitText : MonoBehaviour
    {
        TextMeshProUGUI _text;
        [SerializeField] DefenderManager defenderManager;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            MarketManager.Instance.DefenderPlacementLimit.OnChange += OnDefenderPlacementChanged;
            defenderManager.OnPlacementChange += OnDefenderPlacementChanged;
            defenderManager.OnDefenderChange += OnDefenderChange;
            RoundManager
                .Instance
                .AddOnConfigureStatesEvent(AddOnRoundStart);
            
        }

        private void OnDisable()
        {
            MarketManager.Instance.DefenderPlacementLimit.OnChange -= OnDefenderPlacementChanged;
            defenderManager.OnPlacementChange -= OnDefenderPlacementChanged;
            defenderManager.OnDefenderChange -= OnDefenderChange;
            RoundManager.Instance
                .GetStateBase(PhaseType.Maintenance)
                .Event
                .Remove(StateBaseEventType.Enter,OnRoundStart);
        }

        private void AddOnRoundStart(object _)
        {
            RoundManager.Instance
                .GetStateBase(PhaseType.Maintenance)
                .Event
                .Add(StateBaseEventType.Enter,OnRoundStart);
        }

        private void OnRoundStart(PhaseType _, StateBaseEventType _2)
        {
            RefreshText(
                defenderManager.GetPlacementCount(Placement.BattleArea),
                MarketManager.Instance.DefenderPlacementLimit.Value
            );
        }

        /// <summary>
        /// 수호자가 배치될 때
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="placement"></param>
        private void OnDefenderPlacementChanged(Defender defender, Placement placement)
        {
            RefreshText(
                defenderManager.GetPlacementCount(Placement.BattleArea), 
                MarketManager.Instance.DefenderPlacementLimit.Value
            );
        }

        /// <summary>
        /// 수호자 배치 상한이 변경될 때
        /// </summary>
        /// <param name="limit"></param>
        private void OnDefenderPlacementChanged(int limit)
        {
            RefreshText(
                defenderManager.GetPlacementCount(Placement.BattleArea), 
                limit
            );
        }

        private void OnDefenderChange(Defender defender, DefenderChanges changes)
        {
            RefreshText(
                defenderManager.GetPlacementCount(Placement.BattleArea), 
                MarketManager.Instance.DefenderPlacementLimit.Value
            );
        }

        private void RefreshText(int placeCount, int limit)
        {
            _text.text = $"{placeCount}/{limit}";
        }
    }
}
