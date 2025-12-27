using System;
using Common.Scripts.GlobalEventBus;
using Common.Scripts.Rxs;
using Scenes.Battle.Feature.Events;
using UnityEngine;

namespace Scenes.Battle.Feature.LifeCrystals
{
    public class LifeCrystalManager : MonoBehaviour
    {
        public RxValue<int> MaxLifePoint;
        public RxValue<int> CurrentLifePoint;

        [SerializeField] private float maxLifeBarWidth;
        [SerializeField] private RectTransform lifeBar;
        

        private void Awake()
        {
            MaxLifePoint = new RxValue<int>(100);
            CurrentLifePoint = new RxValue<int>(MaxLifePoint.Value);

            MaxLifePoint.OnChange += UpdateUI;
            CurrentLifePoint.OnChange += UpdateUI;
        }

        private void OnEnable()
        {
            UpdateUI(0);
        }

        public void ChangeLifePoint(int point)
        {
            if (CurrentLifePoint.Value > 0)
            {
                CurrentLifePoint.Value += point;
                
                GlobalEventBus.Publish<OnLifeCrystalPointChangedEventDto>(
                    new OnLifeCrystalPointChangedEventDto(CurrentLifePoint.Value)
                );

                // 생명 수정 포인트가 0이 되었을 때
                if (CurrentLifePoint.Value <= 0)
                {
                    CurrentLifePoint.Value = 0;
                    
                    GlobalEventBus.Publish<OnLifeCrystalDestroyEventDto>(
                        new OnLifeCrystalDestroyEventDto()    
                    );
                }
            }
        }

        private void UpdateUI(int _)
        {
            lifeBar.sizeDelta = new Vector2(
                maxLifeBarWidth * (CurrentLifePoint.Value / (float)MaxLifePoint.Value), 
                lifeBar.sizeDelta.y
            );
        }
    }
}
