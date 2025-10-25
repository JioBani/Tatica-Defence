using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Scripts.DynamicRepeater
{
    public sealed class DynamicRepeater : IDisposable
    {
        private readonly Func<UniTask> _job;           // 매 틱 실행할 비동기 작업
        private readonly Func<TimeSpan> _intervalNow;  // 현재 간격을 제공(외부에서 값만 변경)
        private CancellationTokenSource _stopCts;
        private bool _running;

        /// <param name="intervalNow">현재 주기를 반환하는 함수(예: () => TimeSpan.FromSeconds(currentSec))</param>
        /// <param name="job">매 틱 실행할 작업</param>
        /// <param name="externalToken">호스트 수명과 연동할 토큰(옵션)</param>
        public DynamicRepeater(Func<TimeSpan> intervalNow, Func<UniTask> job, CancellationToken externalToken = default)
        {
            _intervalNow = intervalNow ?? throw new ArgumentNullException(nameof(intervalNow));
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _stopCts = externalToken.CanBeCanceled
                ? CancellationTokenSource.CreateLinkedTokenSource(default)
                : new CancellationTokenSource();
        }

        public void Start(CancellationToken externalToken = default)
        {
            Debug.Log($"is running :  {_running}");
            if (_running) return;
            _running = true;
            _stopCts = externalToken.CanBeCanceled
                ? CancellationTokenSource.CreateLinkedTokenSource(externalToken)
                : new CancellationTokenSource();
            RunAsync(_stopCts.Token).Forget();
        }

        public void Stop()
        {
            if (!_running) return;
            _stopCts.Cancel();
        }

        public void Dispose()
        {
            Stop();
            _stopCts?.Dispose();
            _stopCts = null;
            _running = false;
        }

        private async UniTaskVoid RunAsync(CancellationToken stopToken)
        {
            Debug.Log("RunAsync");
            try
            {
                while (!stopToken.IsCancellationRequested)
                {
                    await _job();

                    var interval = _intervalNow.Invoke();
                    if (interval < TimeSpan.Zero) interval = TimeSpan.Zero;

                    await UniTask.Delay(interval, cancellationToken: stopToken);
                }
            }
            catch (OperationCanceledException) { /* 정상 종료 */ }
            finally
            {
                _running = false;
            }
        }
    }
}
