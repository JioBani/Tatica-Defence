using System;

namespace Common.Scripts.Timers
{
    public class Timer
    {
        public float MaxTime { get; private set; }
        public float CurrentTime { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsTimeOut { get; private set; }

        public Action<float> OnTimeOut;
        
        public Timer(float maxTime)
        {
            MaxTime = maxTime;
            CurrentTime = 0;
        }
        
        public void Start()
        {
            CurrentTime = MaxTime;
            IsRunning =  true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            IsRunning = false;
        }

        public void Resume()
        {
            IsRunning = true;
        }

        public void Update(float deltaTime)
        {
            if (IsRunning)
            {
                CurrentTime -= deltaTime;

                if (CurrentTime <= 0)
                {
                    CurrentTime = 0;
                    IsRunning = false;
                    OnTimeOut?.Invoke(CurrentTime);
                }
            }
        }

        public void Dispose()
        {
            OnTimeOut = null;
        }
    }
}