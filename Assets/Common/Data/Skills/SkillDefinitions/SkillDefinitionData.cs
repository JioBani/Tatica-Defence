using UnityEngine;

namespace Common.Data.Skills
{
    [CreateAssetMenu( menuName = "Skills/SkillDefinitionData", fileName = "SkillDefinitionData", order = 0)]
    public class SkillDefinitionData : ScriptableObject
    {
        //TODO: enum 으로 변경
        [Header("표시 정보 (Identity)")]
        [Tooltip("스킬 ID")]
        [SerializeField] private int id;
        
        [Tooltip("스킬 이름")]
        [SerializeField] private string displayName;
        
        [Tooltip("스킬 설명")]
        [SerializeField] private string description;
        
        [Tooltip("스킬 쿨타임")]
        [SerializeField] private float coolTime;

        [Tooltip("아이콘")]
        [SerializeField] private Sprite icon;
    }
}