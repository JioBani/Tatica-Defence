using System;
using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.Draggable;
using Common.Scripts.Enums;
using Scenes.Battle.Feature.Unit.Skills;
using Scenes.Battle.Feature.Unit.Skills.Contexts;
using Scenes.Battle.Feature.Units.ActionStates;
using Scenes.Battle.Feature.Units.HealthBars;
using Scenes.Battle.Feature.Units.UnitStats.UnitStatSheets;
using Scenes.Battle.Feature.WaitingAreas;
using UnityEngine;

namespace Scenes.Battle.Feature.Units
{
    // TODO: 기능이 많아지는 경우 분리
    public class Unit : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private ActionStateController actionStateController;
        public ActionStateController ActionStateController => actionStateController;
        
        protected Draggable2D Draggable;

        public UnitLoadOutData UnitLoadOutData;
        
        // TODO: unit load out data 로 이동
        public Fraction fraction;
        public readonly UnitStatSheet StatSheet = new();

        public Action<Unit> OnSpawnEvent;
        
        protected ISkillExecutor SkillExecutor;
        
        protected virtual void Awake()
        {
            Draggable = GetComponent<Draggable2D>();

            StatSheet.Health.OnChange += (value) =>
            {
                healthBar.Display(value / StatSheet.MaxHealth.CurrentValue);
            };
        }

        public void MoveToWaitingArea()
        {
            List<ExclusiveDropZone2D> areas = WaitingAreaReferences.Instance.waitingAreas;

            // TODO: ?
            areas.Find((zone) =>
            {
                if (zone.occupant == null)
                {
                    Draggable.MoveToDropZone(zone);
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        // UnitGenerator 에 의해 소환되었을 때
        public void SetSpawn(UnitLoadOutData unitLoadOutData)
        {
            //_unitLoadOutData = unitLoadOutData;
            UnitLoadOutData = unitLoadOutData;
            StatSheet.Init(unitLoadOutData.Stats);
            SkillExecutor = SkillFactory.Instance.CreateSkillExecutor(unitLoadOutData.Skill);
            SkillExecutor.Initialize(new SkillInitializeContext());
            
            //TEMP
            GetComponent<SpriteRenderer>().sprite = unitLoadOutData.Unit.Icon;

            OnSpawn(unitLoadOutData);
            OnSpawnEvent?.Invoke(this);
        }

        protected virtual void OnSpawn(UnitLoadOutData unitLoadOutData)
        {
            
        }
    }
}

