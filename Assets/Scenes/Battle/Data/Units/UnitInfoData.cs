using Scenes.Battle.Scripts.Unit.Skills;
using UnityEngine;

namespace Scenes.Battle.Data.Units
{
    //TODO: 유닛 정보와 스탯을 분리
    [CreateAssetMenu(menuName = "Units/Unit Data", fileName = "UnitData")]
    public class UnitInfoData : ScriptableObject
    {
        [Header("표시 정보 (Identity)")]
        [Tooltip("게임 내에 표시될 유닛 이름")]
        [SerializeField] private string displayName = "New Unit";

        [Tooltip("UI 등에서 사용할 유닛 아이콘")]
        [SerializeField] private Sprite icon;

        [Header("기본 능력치 (Base Stats)")]
        [Tooltip("유닛의 시작 레벨 (최소 1)")]
        [Min(1)]  public int level = 1;

        [Tooltip("최대 체력(HP). 0 이하가 되면 전투불능")]
        [Min(1)]  public float maxHP = 100f;

        [Tooltip("기본 공격력(물리 피해의 기반 수치)")]
        [Min(0)]  public float attack = 10f;

        [Tooltip("방어력(받는 물리 피해를 감소)")]
        [Min(0)]  public float defense = 5f;

        [Tooltip("주문력(스킬 피해/효율에 영향)")]
        [Min(0)]  public float abilityPower = 0f;

        [Space]
        [Tooltip("초당 공격 횟수(예: 1.0 = 1초에 1회 공격)")]
        [Min(0f)] public float attackSpeed = 1.0f;

        [Tooltip("이동 속도(m/s 가정). 값이 높을수록 빠르게 이동")]
        [Min(0f)] public float moveSpeed = 3.5f;

        [Space]
        [Tooltip("치명타 확률(0~1). 예: 0.05 = 5%")]
        [Range(0f, 1f)] public float critChance = 0.05f;

        [Tooltip("치명타 피해 배수. 1.5 = 150% 데미지")]
        [Min(1f)] public float critDamage = 1.5f;

        [Space]
        [Tooltip("방어구 관통(고정 수치). 대상의 방어력을 해당 값만큼 무시")]
        [Min(0f)] public float armorPenetration = 0f;

        [Tooltip("마법 관통(고정 수치). 대상의 마법 저항을 무시")]
        [Min(0f)] public float magicPenetration = 0f;

        [Tooltip("생명력 흡수 비율(0~∞). 가한 피해의 일부를 회복")]
        [Min(0f)] public float lifeSteal = 0f;

        [Header("자원 (선택)")]
        [Tooltip("에너지(마나) 최대치")]
        [Min(0f)] public float energyMax = 100f;

        [Tooltip("초당 에너지(마나) 회복량")]
        [Min(0f)] public float energyRegen = 5f;

        [Header("스킬")]
        [Tooltip("보유 스킬 목록. 배열 순서를 우선순위/슬롯으로 사용할 수 있습니다.")]
        public SkillEnum[] skills;

        // 읽기용 프로퍼티
        public string DisplayName => displayName;
        public Sprite Icon => icon;

        // 편의 지표(튜닝 필요)
        public float ApproxPowerScore =>
            maxHP * 0.5f + attack * 3f + defense * 2f + abilityPower * 3f
            + attackSpeed * 10f + moveSpeed * 2f
            + critChance * 50f + (critDamage - 1f) * 20f;
    }
}
