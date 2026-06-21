namespace Scenes.Battle.Feature.Units.UnitStats
{
    /// <summary>능력치의 단위 종류. 값 강제(정수화 여부)와 표시 형식을 함께 가른다.</summary>
    public enum StatUnit
    {
        /// <summary>정수 단위 (예: 공격력·최대 체력).</summary>
        Integer,

        /// <summary>실수 단위 (예: 공격 속도·사거리·이동 속도).</summary>
        Float,

        /// <summary>비율 단위. 0~1로 저장하고 표시 시 ×100% 한다 (예: 치명타 확률·방어력·쿨타임 감소).</summary>
        Percent,

        /// <summary>배수 단위. 표시 시 "×N" 으로 보인다 (예: 치명타 피해 배수).</summary>
        Multiplier,
    }
}
