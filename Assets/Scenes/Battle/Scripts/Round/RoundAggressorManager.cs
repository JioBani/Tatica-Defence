using System;
using System.Threading;
using Common.Data.Rounds;
using Common.Scripts.RepeatX;
using Cysharp.Threading.Tasks;
using Scenes.Battle.Scripts.Unit;
using UnityEngine;

namespace Scenes.Battle.Scripts.Round
{
    public class RoundAggressorManager : MonoBehaviour
    {
        [SerializeField] private UnitGenerator unitGenerator;

        // 한 라운드(Combat 페이즈) 동안의 스폰 작업을 제어할 취소 토큰 소스
        private CancellationTokenSource _roundContext;
        
        private void Awake()
        {
            // Combat 페이즈 시작(Enter) 시 적 스폰 예약 시작
            RoundManager.Instance
                .GetPhase(PhaseType.Combat)
                .phaseEvent
                .Add(PhaseEventType.Enter, (_, _) => GenerateAggressorByRound());
            
            // Combat 페이즈 종료(Exit) 시 진행 중인 스폰 예약/대기 모두 취소
            RoundManager.Instance
                .GetPhase(PhaseType.Combat)
                .phaseEvent
                .Add(PhaseEventType.Exit,  (_, _) => CancelGeneration());
        }

        /// <summary>
        /// 현재 라운드의 스폰 엔트리들을 순회하며 각각의 스폰 예약을 건다.
        /// fire-and-forget 형태로 예약만 걸고 즉시 반환한다.
        /// </summary>
        private void GenerateAggressorByRound()
        {
            // 이전 라운드의 토큰이 남아있을 수 있으므로 새 컨텍스트 생성
            _roundContext = new CancellationTokenSource();
            
            // 현재 라운드 정보 조회 (각 엔트리에는 지연시간, 수량, 유닛 로드아웃 포함)
            RoundInfoData roundInfo = RoundManager.Instance.GetCurrentRoundData();
            
            // 각 스폰 엔트리별로 비동기 예약 실행 (완료를 기다리지 않음)
            foreach (var entry in roundInfo.spawnEntries)
            {
                GenerateAggressor(entry).Forget();
            }
        }

        /// <summary>
        /// 단일 스폰 엔트리를 처리한다.
        /// 1) 지정된 지연시간만큼 대기
        /// 2) 취소되었는지 확인
        /// 3) count 만큼 유닛 생성
        /// </summary>
        private async UniTask GenerateAggressor(RoundInfoData.SpawnEntry entry)
        {
            try
            {
                // 라운드 공용 취소 토큰 (Exit 등 트리거 시 취소됨)
                var token = _roundContext.Token;

                await UniTask.Delay(entry.spawnTime.ToTimeSpan(), cancellationToken: token);

                // 대기 중 취소되었으면 바로 종료
                if (token.IsCancellationRequested) return;
                
                // 해당 엔트리의 수량만큼 유닛 생성
                RepeatX.Times(entry.count, _ => unitGenerator.GenerateAggressor(entry.unitLoadOutData));
            }
            catch (OperationCanceledException)
            {
                // 취소는 정상 흐름이므로 별도 로그 없이 무시
            }
        }

        /// <summary>
        /// 라운드 종료 등 트리거 시 현재 진행 중인 모든 스폰 예약을 취소하고 자원 정리
        /// </summary>
        private void CancelGeneration()
        {
            _roundContext?.Cancel();   // 진행 중(대기 중) 작업들 일괄 취소 신호
            _roundContext?.Dispose();  // 토큰 소스 자원 해제
            _roundContext = null;
        }
    }
}
