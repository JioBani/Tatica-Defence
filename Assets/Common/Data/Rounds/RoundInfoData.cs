using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using UnityEngine;

namespace Common.Data.Rounds
{
    [CreateAssetMenu(menuName = "Rounds/Round", fileName = "RoundData")]
    public class RoundInfoData : ScriptableObject
    {
        public List<SpawnEntry> spawnEntries = new List<SpawnEntry>();
        
        [System.Serializable]
        public class SpawnEntry
        {
            [Tooltip("해당 유닛이 소환될 시간")]
            public int spawnTime;
            
            [Tooltip("소환할 유닛 정의")]
            public UnitLoadOutData unitLoadOutData;

            [Tooltip("해당 유닛을 몇 마리 소환할지")]
            [Min(1)] public int count = 1;
        }
    }
}