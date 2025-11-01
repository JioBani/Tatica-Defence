using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Common.Scripts.UniTaskHandles
{
    public static class UniTaskHandleExtensions
    {
        public static UniTaskHandle ToHandle(this UniTask task)
            => new UniTaskHandle(task.AsTask());

        public static UniTaskHandle<T> ToHandle<T>(this UniTask<T> task)
            => new UniTaskHandle<T>(task.AsTask());

        public static UniTaskStatus ToUniTaskStatus(this Task task)
        {
            if (!task.IsCompleted) return UniTaskStatus.Pending;
            if (task.IsCanceled)   return UniTaskStatus.Canceled;
            if (task.IsFaulted)    return UniTaskStatus.Faulted;
            return UniTaskStatus.Succeeded;
        }
    }

    public sealed class UniTaskHandle
    {
        private readonly Task _task;
        public UniTaskHandle(Task task) => _task = task ?? throw new ArgumentNullException(nameof(task));

        public bool IsCompleted           => _task.IsCompleted;
        public bool IsFaulted             => _task.IsFaulted;
        public bool IsCanceled            => _task.IsCanceled;
        public Exception Exception        => _task.Exception;
        public UniTaskStatus Status       => _task.ToUniTaskStatus();
        public Task AsTask()              => _task;          // 필요하면 한 번만 await
    }

    public sealed class UniTaskHandle<T>
    {
        private readonly Task<T> _task;
        public UniTaskHandle(Task<T> task) => _task = task ?? throw new ArgumentNullException(nameof(task));

        public bool IsCompleted           => _task.IsCompleted;
        public bool IsFaulted             => _task.IsFaulted;
        public bool IsCanceled            => _task.IsCanceled;
        public AggregateException Exception => _task.Exception;
        public UniTaskStatus Status       => ((Task)_task).ToUniTaskStatus();
        public Task<T> AsTask()           => _task;

        public bool TryGetResult(out T value)
        {
            if (_task.Status == TaskStatus.RanToCompletion)
            {
                value = _task.Result;
                return true;
            }
            value = default;
            return false;
        }
    }
}