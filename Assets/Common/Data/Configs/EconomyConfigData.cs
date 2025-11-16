using UnityEngine;

namespace Common.Data.Configs
{
    [CreateAssetMenu(menuName = "GameConfig/EconomyConfig")]
    public class EconomyConfig : ScriptableObject
    {
        [Header("기본 수입")]
        public int baseGoldPerRound = 5;

        [Header("이자 관련")]
        public int goldPerInterestStep = 10;   // 10골드당 이자
        public int interestPerStep = 1;        // 10골드당 1골드
        public int maxInterest = 5;            // 최대 5골드

        [Header("연승/연패 보너스")]
        public int[] winStreakBonusByCount;    // 인덱스 = 연승길이
        public int[] loseStreakBonusByCount;   // 인덱스 = 연패길이

        public int GetRoundStartIncome(int currentGold, int winStreak, int loseStreak)
        {
            int income = baseGoldPerRound;

            // 이자
            int steps = Mathf.Min(currentGold / goldPerInterestStep, maxInterest);
            income += steps * interestPerStep;

            // 연승 보너스
            if (winStreak > 0 && winStreak < winStreakBonusByCount.Length)
                income += winStreakBonusByCount[winStreak];

            // 연패 보너스
            if (loseStreak > 0 && loseStreak < loseStreakBonusByCount.Length)
                income += loseStreakBonusByCount[loseStreak];

            return income;
        }
    }
}

