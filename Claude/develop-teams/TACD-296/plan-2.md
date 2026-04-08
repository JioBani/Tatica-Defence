# Plan 2: 시너지 효과 설명 및 티어별 수치 표시

> 작업정의: TACD-296 단위 2
> 완료 정의: CD-5, CD-6, CD-7

---

## 1. 현재 상태

### SynergyDetailPanel.cs
- Show()에서 아이콘/이름만 바인딩. 효과 설명이나 티어 정보는 표시하지 않음.
- TierList 컨테이너(VerticalLayoutGroup)가 Scene에 존재하지만 SerializeField 참조 없음.

### Scene 구조
```
SynergyDetailPanel
├── Header (HorizontalLayoutGroup) — DetailIcon, DetailName, CloseButton
├── TierList (VerticalLayoutGroup) — 비어있음
└── UnitList (GridLayoutGroup) — 비어있음
```

### 데이터 구조
- `SynergyDefinitionData.Description`: 플레이스홀더 포함 원본 문자열 (예: `"전쟁기계가 내구력을 얻습니다. 체력 50% 이상일 경우 @highDamageReduction*100@%..."`)
- `SynergyDefinitionData.Tiers`: `IReadOnlyList<SynergyTier>`
- `SynergyTier.RequiredCount`: 활성화에 필요한 유닛 수
- `SynergyTier.Constants`: `SerializableDictionary<string, float>` (예: `{lowDamageReduction: 0.18, highDamageReduction: 0.25}`)
- `SynergyActivation.ActiveTier`: `RxValue<SynergyTier?>` — 현재 활성 티어

---

## 2. 변경 계획

### 2.1 SynergyTierRow.cs 신규 생성

경로: `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyTierRow.cs`

전체 설계(plan-1.md)에 정의된 대로 티어 1행을 표시하는 MonoBehaviour.

**SerializeField:**
- `TMP_Text thresholdText` — 필요 카운트 표시 (예: "2")
- `TMP_Text effectText` — 효과 수치 표시 (Constants를 "key: value" 형태로 나열)
- `CanvasGroup canvasGroup` — 활성/비활성 티어 시각적 구분용

**메서드:**
- `Bind(SynergyTier tier, bool isActive)`:
  - `thresholdText.text = tier.RequiredCount.ToString()`
  - `effectText.text` = Constants의 key-value를 한 줄로 포매팅 (예: "lowDamageReduction: 0.18, highDamageReduction: 0.25")
  - 활성 티어: `canvasGroup.alpha = 1f`
  - 비활성 티어: `canvasGroup.alpha = 0.4f`

### 2.2 SynergyDetailPanel.cs 수정

**SerializeField 추가:**
- `TMP_Text descriptionText` — 시너지 효과 설명
- `Transform tierListContainer` — TierList 컨테이너
- `SynergyTierRow tierRowPrefab` — 티어 행 프리팹

**Show() 변경:**
- 기존 아이콘/이름 바인딩 유지
- `descriptionText.text = activation.Definition.Description` (원본 그대로)
- 기존 TierRow 자식들을 모두 파괴
- `activation.Definition.Tiers`를 순회하며 `tierRowPrefab`을 Instantiate → `tierListContainer`에 추가
- 각 행에 `Bind(tier, isActive)` 호출. 활성 판정: `activation.ActiveTier.Value.HasValue && activation.ActiveTier.Value.Value.Tier == tier.Tier`

### 2.3 Scene 변경 (MCP)

**SynergyDetailPanel 하위에 Description 텍스트 추가:**
- Header와 TierList 사이에 `DescriptionText` GameObject 생성 (TMP_Text)

**SynergyTierRow 프리팹 생성:**
- `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/` 경로에 SynergyTierRow 프리팹 생성
- 구조: SynergyTierRow (HorizontalLayoutGroup + CanvasGroup)
  - ThresholdText (TMP_Text) — 카운트 숫자
  - EffectText (TMP_Text) — 효과 수치

**SerializeField 연결:**
- SynergyDetailPanel에 descriptionText, tierListContainer, tierRowPrefab 연결

---

## 3. 완료 정의 대응

| CD | 구현 위치 |
|---|---|
| CD-5 (효과 설명 표시) | SynergyDetailPanel.Show()에서 descriptionText에 Description 원본 바인딩 |
| CD-6 (티어별 카운트/수치 목록) | SynergyDetailPanel.Show()에서 SynergyTierRow 동적 생성, Bind()로 RequiredCount + Constants 표시 |
| CD-7 (활성/비활성 시각적 구분) | SynergyTierRow.Bind()에서 CanvasGroup.alpha로 구분 (1.0 vs 0.4) |

---

## 4. 변경 파일 목록

1. `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyTierRow.cs` (신규)
2. `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyDetailPanel.cs` (수정)
3. Scene: `Canvas/SynergyInfoPanel/SynergyDetailPanel` 하위에 DescriptionText 추가 (MCP)
4. 프리팹: SynergyTierRow 프리팹 생성 (MCP)
