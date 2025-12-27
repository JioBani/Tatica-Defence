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
    public class DefenderPlacementLimitText : MonoBehaviour, IStateListener<PhaseType>
    {
        TextMeshProUGUI _text;
        [SerializeField] DefenderManager defenderManager;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();

            // IStateListener 등록
            RoundManager.Instance.RegisterListener(this);
        }

        private void OnEnable()
        {
            MarketManager.Instance.DefenderPlacementLimit.OnChange += OnDefenderPlacementChanged;
            defenderManager.OnPlacementChange += OnDefenderPlacementChanged;
            defenderManager.OnDefenderChange += OnDefenderChange;
        }

        private void OnDisable()
        {
            MarketManager.Instance.DefenderPlacementLimit.OnChange -= OnDefenderPlacementChanged;
            defenderManager.OnPlacementChange -= OnDefenderPlacementChanged;
            defenderManager.OnDefenderChange -= OnDefenderChange;
        }

        // IStateListener 명시적 구현
        void IStateListener<PhaseType>.OnStateEnter(PhaseType phaseType)
        {
            if (phaseType == PhaseType.Maintenance)
            {
                OnRoundStart();
            }
        }

        void IStateListener<PhaseType>.OnStateRun(PhaseType phaseType)
        {
            // Run 단계에서는 특별한 동작 없음
        }

        void IStateListener<PhaseType>.OnStateExit(PhaseType phaseType)
        {
            // Exit 단계에서는 특별한 동작 없음
        }

        private void OnDestroy()
        {
            RoundManager.Instance.UnregisterListener(this);
        }

        private void OnRoundStart()
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
