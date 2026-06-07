# QA 자동화 (TACD-304)

LLM 에이전트가 게임을 **관측·제어·셋업**하도록 게임 내부를 노출하는 에디터 전용 QA 계층.
**명세(WHAT)와 구현(HOW)을 분리**한다 — 커맨드 계약은 `qa-spec.json`(단일 진실)에, 동작은 `QaApi`에. 둘의 어긋남은 드리프트 검증기가 막는다.

> 커맨드를 추가/변경할 때마다 **`qa-spec.json` 과 `QaApi` 를 함께** 갱신한다. 저장 시 드리프트 검증기가 자동으로 정합을 검사한다.

## 1. 아키텍처 (한 줄)

```
에이전트 → qa_index/qa_call (네이티브 게이트웨이 2도구, QaDispatch.cs) → QaApi 파사드(통로 독립 순수 로직) → 게임(SceneSingleton.Instance / Find)
                         ↑ 계약(WHAT) = qa-spec.json            드리프트 검증기(QaSpecValidator)가 둘의 정합 강제
```

- **노출 도구는 2개로 고정**: `qa_index`(커맨드 발견) · `qa_call`(커맨드 디스패치). 개별 커맨드는 `qa_call` 의 `args` 안에서 자유 → 도구 시그니처가 안 흔들려 스키마 stickiness 무관.
- **명세 분리(D-7)**: 커맨드 목록·인자 스키마·반환 계약은 전부 `qa-spec.json`. `QaApi` 는 동작만(설명 어트리뷰트 없음). 노출 경계 = **스펙 등재**(스펙에 없으면 `qa_call` 로 호출 불가).
- **단방향 의존(QA→게임)**: 게임 코드는 QA 를 전혀 모른다. 게임 매니저는 `SceneSingleton.Instance`(또는 비-싱글톤은 `FindAnyObjectByType`)로 접근. 관측에 게임 내부가 필요하면 게임 매니저에 **public read-only 게터**를 더한다(리플렉션 지양 — 견고·자기문서화).
- **파사드 분리**: 로직은 `QaApi`(MCP 비종속), MCP 어댑팅(JObject 파싱·검증·라우팅·봉투)은 `QaDispatch`. 보조 경로 `execute_code` 도 `QaApi.*` 를 직접 호출할 수 있다.
- **공통 봉투**: 성공 `{ ok:true, data }` / 실패 `{ ok:false, error }`. (벤더 MCP 가 이를 다시 `{success, data, ...}` 로 감싸므로 에이전트에는 `data.ok`/`data.data` 로 보인다.)

## 2. 새 커맨드 추가 레시피 (정본)

**핵심: `qa-spec.json` 에 커맨드 등재 + `QaApi` 에 동명 메서드 1개. 디스패치/도구 재등록 없음.**

1. **`qa-spec.json` 의 `commands` 에 항목 추가** — `description` / `category` / `slice` / `parameters`(JSON Schema) / `returns`. 반복 객체는 `schemas.output`/`schemas.input` 에 정의하고 `$ref`. 커맨드 키 = QaApi 메서드명(라우팅 키).
2. **`QaApi` 에 동명 public static 메서드 추가** — 시그니처를 스펙 `parameters` 와 일치(이름·타입·required). 반환은 스펙 `returns` 스키마에 맞는 DTO(`Dictionary<string,object>` 평탄 포맷). 내부는 기존 게임 경로를 래핑(MCP 의존 금지).
3. **저장** → 드리프트 검증기가 자동 검사. 메뉴 `Tools/QA/Validate Spec` 로 수동 실행도 가능.

### 규칙 (디스패치·검증기가 강제)
- **커맨드명(=메서드명)은 유일**. 라우팅·검증이 이름 기준(대소문자 무시).
- **인자 매핑**: `qa_call` 이 `args`(맵)를 메서드 파라미터에 이름 기준·대소문자 무시로 `JToken.ToObject(타입)` 변환. 누락 시 기본값, 없으면 "필수 인자 누락" 에러. 명세 `required` 누락은 호출 전 차단.
- **드리프트 검증**: slice=S1 커맨드는 동명 메서드 존재 강제. 메서드 가진 커맨드는 인자(이름·타입·required) 정합. QaApi public static 메서드는 전부 스펙 등재(노출 불가 메서드 금지). `returns` 는 문서용(미검증).

> canonical 예시: `QaApi.PlaceUnit(int unitInstanceId, QaGridPosition position)` ([`QaApi.cs`](Editor/QaApi.cs)) ↔ 스펙 `PlaceUnit` → `qa_call{command:"PlaceUnit", args:{unitInstanceId:123, position:{lane:0, column:1}}}`.
> "클래스 불문 임의 값 조회"는 신규 커맨드를 만들지 말고 MCP 기본 도구(`unity_reflect`/`execute_code`)로 드릴다운(도구로 노출 안 된 자유 탐색 전용).

## 3. 동작 규약 (명세 일부)

- **입력 = 실제 입력 이벤트**: 버튼은 핸들러/`onClick.Invoke`, 배치/이동은 `Draggable2D.MoveToDropZone`(좌표 X — D-4). 매니저 직접 호출로 가로지르지 않는다. 단 배치 규칙은 `DropZone2D.CanAccept` 로 먼저 검사(MoveToDropZone 자체는 규칙 미검사).
- **관측 = 매니저 상태 직접 조회**, 패널 독립.
- **전장 격자(lane,column)**: 코드에 1급 격자 타입은 없으나 씬은 4 레인×8 열(32 `DefenderSideSell`) 규칙 격자. QA 가 드롭존 월드좌표를 정렬해 매핑(lane=Y 오름차순, column=X 오름차순/왼쪽=0). 컨벤션 정본은 `qa-spec.json` 의 `GridPosition` 설명.
- **식별자**: instanceId=정수(`GameObject.GetInstanceID()`, 살아있는 동안 유일) / definitionId=문자열(`UnitLoadOutData.ID.ToString()`).
- **셋업/주입**: 일관된 QA 출처 키, 조용한 실패 금지(`Debug.LogWarning`/`LogError`).
- **토큰 절약 반환**: 평탄 요약 우선, 거대 객체 통째 직렬화 지양. 스크린샷 등 큰 자원은 **경로만** 반환.

## 4. 어셈블리 / 빌드 배제

- QA 코드·`qa-spec.json` 은 `Assets/QA/Editor/` 에 둔다 → **`Assembly-CSharp-Editor`(에디터 전용)** → 플레이어 빌드에서 **Unity 가 자동 제외**. `#if`·별도 asmdef 불필요. 게임은 QA 를 0 참조.
- 관측용으로 게임 매니저에 더한 **public read-only 게터**(예: `RoundAggressorManager.Aggressors`, `MarketManager.CurrentSlots`, `MarketUiManager.IsOpen`, `DefenderSlot.Index`, `SummonerManager.SpawnedSummoners`)는 read 전용이라 게임 동작·빌드에 영향 없음(QA→게임 단방향 유지).
- (이행 메모) 장차 **빌드 통로 QA(AR-3)** 부활 시 파사드를 런타임 어셈블리 + `#if DEVELOPMENT_BUILD`/`QA_ENABLED` 로 이전(게임 asmdef 화 동반).

## 5. 런타임 카탈로그 (D-5 — 에이전트용)

- **`qa_index`**(무인자): 사용 가능한 커맨드 압축 목록(`Name(p:type) [slice] — 설명`). **`qa_index{command}`**: 그 커맨드 풀 스키마(parameters·returns).
- **`qa_call{command, args}`**: 인덱스의 이름으로 호출. `args` 는 인덱스/상세가 알려준 인자 맵.
- 사용 흐름: `qa_index` 발견 → 관측 → 행동 → 재관측 → (필요 시)`Screenshot`. **전장 진입·Play 는 사람이 수동**(SceneData 셋업 필요). 전체 덤프보다 요약 스냅샷 우선, 도구로 없는 자유 디버깅만 `execute_code`/리플렉션.

---

- 측정 근거·함정 메모(`execute_code` C# 6 제약, 스크린샷 플레이모드 필수, Unity 객체 통째 직렬화 금지): [`기술조사.md`](../../Claude/develop/TACD-304/기술조사.md) Part B·C-1·C-7.
- 작업 정의·유지 원칙: [`T6-지식정리.md`](../../Claude/develop/TACD-304/작업/T6-지식정리.md). 커맨드 명세: [`qa-spec.json`](Editor/qa-spec.json).
