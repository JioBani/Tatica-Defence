using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Scenes.Battle.Data.Rounds;
using Scenes.Battle.Scripts.Round;
using TMPro;
using UnityEngine;

namespace Scenes.Battle.Scripts.Ui
{
    public class SwitchViewManager : MonoBehaviour
    {
        [SerializeField] private bool isEnemySideView;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private GameObject roundPanel;
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI roundInfoText;

        public Action<bool> switchViewEvent;

        Camera _mainCamera;
        Phase _maintenancePhase;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
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
                _mainCamera.transform.DOMoveY(6, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(UpdateType.Late, isIndependentUpdate: false);
                
                buttonText.text = "아군 진영";
                roundPanel.SetActive(true);
            }
            else
            {
                _mainCamera.transform.DOMoveY(0, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(UpdateType.Late, isIndependentUpdate: false);

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

            Dictionary<string, int> enemyInfos = roundInfoData.waves
                .SelectMany(wave => wave.spawns)
                .GroupBy(spawn => spawn.unitInfo.name)
                .ToDictionary(group => group.Key, group => group.Sum(s => s.count));

            roundInfoText.text = String.Empty;
            
            foreach (var pair in enemyInfos)
            {
                roundInfoText.text += $"{pair.Key} x {pair.Value}, ";
            }
        }
    }
}


