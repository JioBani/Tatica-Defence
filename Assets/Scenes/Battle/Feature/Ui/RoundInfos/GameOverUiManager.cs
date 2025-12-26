using System;
using Common.Scripts.GlobalEventBus;
using Scenes.Battle.Feature.Events.RoundEvents;
using UnityEngine;

namespace Scenes.Battle.Feature.Ui.RoundInfos
{
    public class GameOverUiManager : MonoBehaviour
    {
        [SerializeField] GameObject gameOverPanel;
        
        private void OnEnable()
        {
            GlobalEventBus.Subscribe<OnGameOverEventDto>(OnGameOver);
            gameOverPanel.SetActive(false);
        }

        private void OnDisable()
        {
            GlobalEventBus.Unsubscribe<OnGameOverEventDto>(OnGameOver);
        }

        private void OnGameOver(OnGameOverEventDto _)
        {
            gameOverPanel.SetActive(true);
        }
    }
}   
