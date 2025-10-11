using System.Collections.Generic;
using Common.Data.Units.UnitLoadOuts;
using UnityEngine;

namespace Common.Data.Rounds
{
    [CreateAssetMenu(menuName = "Rounds/Round", fileName = "RoundData")]
    public class RoundInfoData : ScriptableObject
    {
        [Header("라운드 정보")]
        [Tooltip("라운드 번호(1부터 시작 권장)")]
        [Min(1)] public int round = 1;

        [Header("웨이브 목록")]
        [Tooltip("이 라운드에서 순서대로 진행될 웨이브들")]
        public List<Wave> waves = new();

        [System.Serializable]
        public class Wave
        {
            [Tooltip("이 웨이브에서 소환할 유닛과 수량 목록")]
            public List<SpawnEntry> spawns = new();
        }

        [System.Serializable]
        public class SpawnEntry
        {
            [Tooltip("소환할 유닛 정의")]
            public UnitLoadOutData unitLoadOutData;

            [Tooltip("해당 유닛을 몇 마리 소환할지")]
            [Min(1)] public int count = 1;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (waves == null) waves = new List<Wave>();

            // 기본 정합성 체크: null 유닛 제거, count 하한 보정
            foreach (var wave in waves)
            {
                if (wave?.spawns == null) continue;

                wave.spawns.RemoveAll(e => e == null);
                foreach (var e in wave.spawns)
                {
                    if (e == null) continue;
                    if (e.count < 1) e.count = 1;
                }
            }
        }
#endif
    }
}