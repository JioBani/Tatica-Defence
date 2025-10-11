using Common.Data.Skills;
using Common.Data.Skills.SkillDefinitions;
using Common.Data.Units.UnitDefinitions;
using UnityEngine;

namespace Common.Data.Units.UnitLoadOuts
{
    [CreateAssetMenu( menuName = "Units/UnitLoadOutData", fileName = "UnitLoadOutData", order = 0)]
    public class UnitLoadOutData : ScriptableObject
    {
        //TODO: enum 으로 변경
        [Header("표시 정보 (Identity)")]
        [Tooltip("로드아웃 ID")]
        [SerializeField] private int id;
        public int ID => id;
        
        [Tooltip("유닛 정의 데이터")]
        [SerializeField] private UnitDefinitionData unit;
        public UnitDefinitionData  Unit => unit;
        
        [Tooltip("스킬 정의 데이터")]
        [SerializeField] private SkillDefinitionData skill;
        public SkillDefinitionData  Skill => skill;
    }
}