# Plan 1: 상세 패널 기본 구조 및 열기/닫기

> 작업정의: TACD-296 단위 1
> 완료 정의: CD-1, CD-2, CD-3, CD-4, CD-11

---

## 1. 현재 상태

### SynergyDetailPanel.cs
- `Show(SynergyActivation)`: `gameObject.SetActive(true)`만 수행. 데이터 바인딩 없음.
- `Hide()`: `gameObject.SetActive(false)`만 수행.
- 토글 로직 없음.

### SynergyInfoPanel.cs
- `HandleIndicatorClicked(SynergyActivation)`: 항상 `detailPanel.Show(activation)` 호출.
- 같은 시너지 재클릭 시 닫히지 않고, 다른 시너지 클릭 시 교체 로직도 없음.

### Scene 구조
- `Canvas/SynergyInfoPanel/SynergyDetailPanel` — 비활성, 자식 없음, RectTransform + SynergyDetailPanel 스크립트만 존재.

---

## 2. 변경 계획

### 2.1 SynergyDetailPanel.cs 수정

현재 껍데기인 SynergyDetailPanel에 다음을 추가한다:

**SerializeField 추가:**
- `Image detailIcon` — 시너지 아이콘
- `TMP_Text detailName` — 시너지 이름
- `Button closeButton` — 닫기 버튼

**필드 추가:**
- `SynergyActivation _currentActivation` — 현재 표시 중인 시너지 (토글 판정용)

**메서드 변경:**
- `Show(SynergyActivation activation)`:
  - `_currentActivation`에 저장
  - `detailIcon.sprite = activation.Definition.Icon`
  - `detailName.text = activation.Definition.DisplayName`
  - `gameObject.SetActive(true)`
- `Hide()`: 기존 유지 + `_currentActivation = null`
- 프로퍼티 추가: `CurrentActivation` — 외부에서 현재 표시 중인 시너지를 읽기 위함

**Awake:**
- `closeButton.onClick.AddListener` → `Hide()` 호출

### 2.2 SynergyInfoPanel.cs 수정

`HandleIndicatorClicked`에 토글/교체 로직을 추가한다:

```
HandleIndicatorClicked(SynergyActivation activation):
  if 패널이 활성 상태이고 같은 시너지를 클릭 → detailPanel.Hide()
  else → detailPanel.Show(activation)
```

- "같은 시너지" 판정: `detailPanel.CurrentActivation == activation` (참조 비교)
- "패널이 활성 상태": `detailPanel.gameObject.activeSelf`

### 2.3 Scene 계층 구조 (MCP로 생성)

SynergyDetailPanel 하위에 다음 GameObject를 생성한다:

```
SynergyDetailPanel
├── Header
│   ├── DetailIcon       [Image] — 시너지 아이콘
│   ├── DetailName       [TextMeshProUGUI] — 시너지 이름 (Maplestory Bold SDF)
│   └── CloseButton      [Button + Image] — 닫기 버튼
├── TierList             [VerticalLayoutGroup] — (단위 2에서 사용, 지금은 빈 컨테이너)
└── UnitList             [GridLayoutGroup] — (단위 3에서 사용, 지금은 빈 컨테이너)
```

- SynergyDetailPanel에 VerticalLayoutGroup 또는 적절한 레이아웃을 설정하여 Header / TierList / UnitList를 세로 배치한다.
- Header에 HorizontalLayoutGroup을 적용하여 아이콘, 이름, 닫기 버튼을 가로 배치한다.
- 기본 비활성(`SetActive(false)`) 상태를 유지한다.

---

## 3. 완료 정의 대응

| CD | 구현 위치 |
|---|---|
| CD-1 (클릭 시 패널 열기) | SynergyInfoPanel.HandleIndicatorClicked → detailPanel.Show() |
| CD-2 (같은 시너지 재클릭 시 닫기) | SynergyInfoPanel.HandleIndicatorClicked 토글 로직 |
| CD-3 (다른 시너지 클릭 시 교체) | SynergyInfoPanel.HandleIndicatorClicked else 분기 → Show(새 activation) |
| CD-4 (아이콘과 이름 표시) | SynergyDetailPanel.Show()에서 detailIcon, detailName 바인딩 |
| CD-11 (닫기 버튼) | SynergyDetailPanel.Awake()에서 closeButton.onClick → Hide() |

---

## 4. 변경 파일 목록

1. `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyDetailPanel.cs` (수정)
2. `Assets/Scenes/Battle/Feature/Ui/SynergyInfo/Scripts/SynergyInfoPanel.cs` (수정)
3. Scene: `Canvas/SynergyInfoPanel/SynergyDetailPanel` 하위 GameObject 생성 (MCP)
