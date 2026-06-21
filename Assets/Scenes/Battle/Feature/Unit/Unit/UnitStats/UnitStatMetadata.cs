using System;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.UnitStats
{
    /// <summary>한 능력치 종류의 값 규칙(단위·범위·기본값·합산 방식)을 담고, 최종값을 정의에 맞게 정규화한다.</summary>
    public readonly struct UnitStatMetadata
    {
        /// <summary>단위 종류. 표시 형식과 정수화 여부를 결정한다.</summary>
        public StatUnit Unit { get; }

        /// <summary>최종값 하한 (포함).</summary>
        public float Min { get; }

        /// <summary>최종값 상한 (포함). 무제한이면 float.PositiveInfinity.</summary>
        public float Max { get; }

        /// <summary>성급 기준값 미입력 시 사용할 기본값.</summary>
        public float Default { get; }

        /// <summary>수정자 합산 방식.</summary>
        public StatCalculationMode CalculationMode { get; }

        public UnitStatMetadata(StatUnit unit, float min, float max, float defaultValue,
                                StatCalculationMode mode = StatCalculationMode.Additive)
        {
            // 정의 자체의 오류(상한 < 하한)는 무음 통과시키지 않고 즉시 드러낸다.
            if (max < min)
            {
                throw new ArgumentException(
                    $"[UnitStatMetadata] 상한({max})이 하한({min})보다 작습니다. 단위={unit}");
            }

            Unit = unit;
            Min = min;
            Max = max;
            Default = defaultValue;
            CalculationMode = mode;
        }

        /// <summary>원시 최종값을 단위·범위 정의에 맞게 정규화한다(범위 클램프 + 정수 단위면 정수화).</summary>
        public float Normalize(float rawValue)
        {
            float clamped = Mathf.Clamp(rawValue, Min, Max);

            // 정수 단위 능력치(공격력·최대 체력)의 최종값은 정수여야 한다.
            if (Unit == StatUnit.Integer)
            {
                clamped = Mathf.Round(clamped);
            }

            return clamped;
        }
    }
}
