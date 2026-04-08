# Plan 3: 소환수 목록 및 배치 상태 표시

> 작업정의: TACD-296 단위 3
> 완료 정의: CD-8, CD-9, CD-10

---

## 1. 현재 상태

### SynergyDetailPanel.cs
- Show()에서 아이콘/이름/설명/티어 행을 바인딩. 소환수 목록은 미구현.
- UnitList 컨테이너(GridLayoutGroup)가 Scene에 존재하지만 참조 없음.

### SynergyManager.cs
- `_unitSynergyMap`: `Dictionary<UnitLoadOutData, SynergyDefinitionData>` (소환수 → 시너지 방향)
- 역방향 조회(시너지 → 소환수 목록)를 위한 공개 API 없음.

### Defender.cs
- `Placement` 프로퍼티: `WaitingArea` 또는 `BattleArea`
- `UnitLoadOutData`에 접근 가능 (Unit.Unit.Icon으로 아이콘 접근)

### DefenderManager.cs
- 싱글톤이 아님. SerializeField로 주입하는 패턴.
- `Defenders`: 현재 존재하는 모든 Defender 목록.

### 이벤트
- `OnDefenderPlacementChangedEventDto`: Defender 배치 변경 시 발행 (Defender, Placement)
- `OnDefenderChangedEventDto`: Defender Spawn/Despawn 시 발행

---

## 2. 변경 계획

### 2.1 SynergyManager.cs 수정

시너지 → 소환수 목록 역방향 조회 메서드를 추가한다.

```
GetUnitsForSynergy(SynergyDefinitionData definition) → List<UnitLoadOutData>
```

`_unitSynergyMap`을 순회하여 해당 시너지에 속하는 UnitLoadOutData 목록을 반환한다.

### 2.2 SynergyUnitSlot.cs 신규 생성

경로: `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyUnitSlot.cs`

전체 설계(plan-1.md)에 정의된 대로 소환수 아이콘 슬롯.

**SerializeField:**
- `Image unitIcon` — 소환수 아이콘
- `CanvasGroup canvasGroup` — 활성/비활성 시각적 구분용

**필드:**
- `UnitLoadOutData _unitLoadOut` — 바인딩된 소환수 데이터

**프로퍼티:**
- `UnitLoadOutData UnitLoadOut` — 외부에서 현재 바인딩된 소환수 조회

**메서드:**
- `Bind(UnitLoadOutData unitLoadOut)`: 아이콘 설정
- `SetActive(bool isActive)`: canvasGroup.alpha로 배치(1.0)/미배치(0.4) 구분

### 2.3 SynergyDetailPanel.cs 수정

**SerializeField 추가:**
- `Transform unitListContainer` — UnitList 컨테이너
- `SynergyUnitSlot unitSlotPrefab` — 소환수 슬롯 프리팹
- `DefenderManager defenderManager` — Defender 목록 접근용

**Show() 변경:**
- 기존 바인딩 유지
- `BindUnitSlots(activation)` 호출 추가

**신규 메서드:**
- `BindUnitSlots(SynergyActivation activation)`:
  - 기존 슬롯 자식들을 모두 파괴
  - `SynergyManager.Instance.GetUnitsForSynergy(activation.Definition)`으로 소환수 목록 조회
  - 각 UnitLoadOutData에 대해 `unitSlotPrefab` Instantiate → `unitListContainer`에 추가
  - `Bind(unitLoadOut)` 호출
  - `RefreshUnitSlotStates()` 호출하여 배치 상태 갱신

- `RefreshUnitSlotStates()`:
  - `unitListContainer` 자식의 모든 SynergyUnitSlot을 순회
  - 각 슬롯의 UnitLoadOut과 일치하는 Defender가 BattleArea에 있는지 판별
  - `slot.SetActive(isDeployed)` 호출

**이벤트 구독 (OnEnable/OnDisable):**
- `OnDefenderPlacementChangedEventDto` 구독 → `RefreshUnitSlotStates()` 호출 (CD-10)
- `OnDefenderChangedEventDto` 구독 → `RefreshUnitSlotStates()` 호출 (Spawn/Despawn 시 갱신)

### 2.4 Scene/프리팹 변경

**SynergyUnitSlot 프리팹 생성:**
- 구조: SynergyUnitSlot (CanvasGroup + SynergyUnitSlot 스크립트)
  - UnitIcon (Image)
- `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/SynergyUnitSlot.prefab`에 저장

**SynergyDetailPanel SerializeField 연결:**
- unitListContainer → UnitList GameObject
- unitSlotPrefab → SynergyUnitSlot 프리팹
- defenderManager → Scene의 DefenderManager

---

## 3. 완료 정의 대응

| CD | 구현 위치 |
|---|---|
| CD-8 (소환수 목록 아이콘 표시) | SynergyDetailPanel.BindUnitSlots()에서 SynergyUnitSlot 동적 생성, SynergyManager.GetUnitsForSynergy()로 조회 |
| CD-9 (배치/미배치 시각적 구분) | SynergyUnitSlot.SetActive()에서 CanvasGroup.alpha 1.0/0.4, RefreshUnitSlotStates()에서 BattleArea 판별 |
| CD-10 (실시간 갱신) | OnDefenderPlacementChangedEventDto, OnDefenderChangedEventDto 구독 → RefreshUnitSlotStates() |

---

## 4. 변경 파일 목록

1. `Assets/Scenes/Battle/Feature/Synergy/Scripts/SynergyManager.cs` (수정) — GetUnitsForSynergy() 추가
2. `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyUnitSlot.cs` (신규) — 소환수 아이콘 슬롯
3. `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyDetailPanel.cs` (수정) — 소환수 목록 바인딩 및 실시간 갱신
4. 프리팹: SynergyUnitSlot.prefab (신규)
5. Scene: SerializeField 연결 (unitListContainer, unitSlotPrefab, defenderManager)
