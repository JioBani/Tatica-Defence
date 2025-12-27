# Context for Next Session

## 2025-12-28

### DOTween 연속 실행 시
- `DOComplete()` 사용 ✅ (목표값까지 즉시 완료 후 다음 시작)
- `DOKill()` 피하기 ❌ (부자연스러운 중단)

### 비동기 지연 처리
- UniTask + CancellationTokenSource 사용
- OnDisable에서 Cancel() + Dispose() 필수
