---
name: qa-wait
description: "QA 자동화에서 게임이 특정 조건에 도달할 때까지 메인을 블로킹하지 않고 기다린다. 블로킹 RunUntil 호출을 일회용 Haiku 서브에이전트(백그라운드)에 떠넘기고, 그 완료가 메인을 재호출하게 한다. 조건셋·타임아웃을 인자로 받는다. (TACD-304)"
---

# qa-wait — 비블로킹 조건 대기 (Haiku 릴레이)

게임 조건 시점("전투 끝나면/적 N 이하면…")에 관측·입력하려고 그 시점까지 기다릴 때 쓴다.
메인이 직접 `RunUntil`을 부르면 그 동안 블로킹되므로, **일회용 Haiku 워처**에게 대기를 떠넘긴다 → 워처가 완료되면 하네스가 메인을 재호출한다.

**전제**: time 카테고리 `RunUntil` 구현되어 있어야 함(미구현이면 대기). 명세 단일 진실 = `Assets/QA/Editor/qa-spec.json`. 게이트웨이 = `qa_call{command, args}`.
RunUntil은 **폴링 기반**(매 프레임 상태 검사, 비침습) · 단일 호출은 Unity 브릿지 30s 캡에 막히므로 `timeoutMs≤28000` · 더 길면 타임아웃마다 재호출(청크 재무장).

## 사용법

**1. 워처 띄우기** — `Agent` 도구: `subagent_type:general-purpose`, `model:haiku`, `run_in_background:true`, prompt = 아래 지시서.

**2. Haiku 지시서** (그대로, `<조건배열>`·`<freezeOnTrigger>`·`<재시도>`만 치환):
```
너는 QA 대기 릴레이다. 아래만 반복한다.
1. qa_call 호출(안 보이면 ToolSearch로 "qa_call" 로드):
   command="RunUntil",
   args={"conditions":<조건배열>, "timeoutMs":28000, "freezeOnTrigger":<freezeOnTrigger>}
2. 결과 판정:
   - triggered=true 또는 에러 → 그 JSON 그대로 출력하고 끝.
   - reason="timeout" → 1로 돌아가 다시 호출(최대 <재시도>회, 소진되면 마지막 timeout JSON 출력하고 끝).
다른 도구·해석·설명 없이 결과 JSON만.
```
예: `<조건배열>`=`[{"type":"CombatEnded"}]`(배열=OR, 먼저 켜지는 것), `<freezeOnTrigger>`=`true`, `<재시도>`=`4`(≈2분).

**3. 깨어난 뒤** — `task-notification`으로 결과(`{triggered, reason, matchedCondition}`) 수신.
- `freezeOnTrigger:true`였으면 게임은 검출 프레임에 **정지 상태**(Haiku 종료 무관) → 관측 커맨드로 조회 후 `Resume`/`SetTimeScale{x:1}`로 재개.
- `freezeOnTrigger:false`였으면 게임은 계속 진행 중(통지만 받은 것).

## 규칙
- Haiku엔 판단 주지 않음: "RunUntil 호출·타임아웃 재무장·결과만." 여러 조건은 `conditions` 배열 하나로(워처 1개).
- **짧고 1회성 대기면 이 스킬 말고 메인이 직접 포그라운드 `qa_call` 호출**이 단순. 이 스킬은 "길거나 병행작업 필요"할 때.
- 정확한 트리거 프레임 정지가 꼭 필요하면 폴링으론 한두 프레임 늦을 수 있음 — 그 땐 이벤트 기반(후순위)이 필요. 지금은 폴링 수용.
