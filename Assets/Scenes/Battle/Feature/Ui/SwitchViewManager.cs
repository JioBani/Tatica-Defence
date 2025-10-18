using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Common.Data.Rounds;
using Scenes.Battle.Feature.Rounds.CameraControl;
using Scenes.Battle.Feature.Rounds;
using TMPro;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds.Ui
{
    public class SwitchViewManager : MonoBehaviour
    {
        [SerializeField] private bool isEnemySideView;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private GameObject roundPanel;
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI roundInfoText;
        [SerializeField] private CameraControlManager cameraControlManager;

        public Action<bool> switchViewEvent;

        Phase _maintenancePhase;
        
        private void Awake()
        {
            RoundManager.Instance
                .GetPhase(PhaseType.Maintenance)
                .phaseEvent
                .Add(PhaseEventType.Enter, OnMaintenanceEnter);
        }
        
        private void OnMaintenanceEnter(PhaseType _, PhaseEventType __) => SetRoundInfo();

        public void SwitchView()
        {
            isEnemySideView = !isEnemySideView;

            if (isEnemySideView)
            {
                cameraControlManager.ShowAggressorSide();
                buttonText.text = "아군 진영";
                roundPanel.SetActive(true);
            }
            else
            {
                cameraControlManager.ShowDefenderSide();
                buttonText.text = "적 진영";
                roundPanel.SetActive(false);
            }
            
            switchViewEvent?.Invoke(isEnemySideView);
        }

        /// <summary>
        /// 라운드 데이터 UI 세팅
        /// </summary>
        void SetRoundInfo()
        {
            RoundInfoData roundInfoData = RoundManager.Instance.GetCurrentRoundData();

            roundText.text = $"{RoundManager.Instance.RoundIndex} 라운드";
            
            Dictionary<string, int> enemyInfos = roundInfoData.spawnEntries
                .GroupBy(spawn => spawn.unitLoadOutData.Unit.DisplayName)
                .ToDictionary(group => group.Key, group => group.Sum(s => s.count));
            
            roundInfoText.text = String.Empty;
            
            foreach (var pair in enemyInfos)
            {
                Debug.Log(pair.Key + " : " + pair.Value);
                roundInfoText.text += $"{pair.Key} x {pair.Value}, ";
            }
        }
    }
}


