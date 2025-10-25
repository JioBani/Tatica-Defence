using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data.Rounds;
using Common.Data.Units.UnitLoadOuts;
using Common.Scripts.ObjectPool;
using Scenes.Battle.Feature.Aggressors;
using Scenes.Battle.Feature.Rounds.Phases;
using Scenes.Battle.Feature.Sells;
using Scenes.Battle.Feature.Units;
using UnityEngine;

namespace Scenes.Battle.Feature.Rounds
{
    public class RoundInfoViewer : MonoBehaviour
    {
        [SerializeField] private UnitGenerator unitGenerator;
        [SerializeField] private AggressorSideSellManager  aggressorSideSellManager;
        
        private RoundInfoData _currentRoundInfo;
        private readonly List<AggressorSample> _samples = new ();

        void Awake()
        {
            RoundManager.Instance
                .GetPhase(PhaseType.Maintenance)
                .phaseEvent
                .Add(PhaseEventType.Enter, (_,_) => ShowRoundInfo());
            
            RoundManager.Instance
                .GetPhase(PhaseType.Maintenance)
                .phaseEvent
                .Add(PhaseEventType.Exit, (_,_) => HideRoundInfo());
        }

        void ShowRoundInfo()
        {
            _currentRoundInfo = RoundManager.Instance.GetCurrentRoundData();
            
            // 유닛 ID 로 그룹화 해서 하나의 유닛당 하나씩만 미리보기 소환
            Dictionary<int, UnitLoadOutData> enemyInfos = _currentRoundInfo.spawnEntries
                .GroupBy(spawn => spawn.unitLoadOutData.Unit.ID)
                .ToDictionary(group => group.Key, group => group.First().unitLoadOutData);
            
            Dictionary<int, int> aggressorCounts = _currentRoundInfo.spawnEntries
                .GroupBy(spawn => spawn.unitLoadOutData.Unit.ID)
                .ToDictionary(group => group.Key, group => group.Sum(e => e.count));
            
            foreach (var pair in enemyInfos)
            {
                AggressorSideSell sell = aggressorSideSellManager.GetEmptySell();

                if (sell != null)
                {
                    Feature.Units.Unit unit = unitGenerator.GenerateAggressorSample(pair.Value);

                    AggressorSample sample = unit.GetComponent<AggressorSample>();
                    
                    sample.SetCount(aggressorCounts[pair.Key]);
                    
                    sell.SetUnit(unit);

                    unit.transform.position = new Vector3(
                        sell.transform.position.x,
                        sell.transform.position.y,
                        unit.transform.position.z
                    );
                    
                    _samples.Add(sample);
                }
            }
        }

        void HideRoundInfo()
        {
            foreach (var sample in _samples)
            {
                sample.GetComponent<Poolable>().DeSpawn();
            }

            _samples.Clear();
        }
    }
}
