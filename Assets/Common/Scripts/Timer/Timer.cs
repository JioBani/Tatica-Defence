using System;

namespace Common.Scripts.Timers
{
    public class Timer
    {
        public float MaxTime { get; private set; }
        public float CurrentTime { get; private set; }
        public bool IsRunning { get; private set; }

        public bool IsTimeOut => CurrentTime <= 0;

        // isTimeOut, MaxTime
        public Action<bool, float> OnTimeOutChange;
        
        public Timer(float maxTime)
        {
            MaxTime = maxTime;
            CurrentTime = 0;
        }
        
        public void Start()
        {
            CurrentTime = MaxTime;
            IsRunning =  true;
            OnTimeOutChange?.Invoke(IsTimeOut, MaxTime);
        }

        public void Stop()
        {
            IsRunning = false;
            OnTimeOutChange?.Invoke(IsTimeOut, MaxTime);
        }

        public void Reset()
        {
            CurrentTime = MaxTime;
            IsRunning = false;
            OnTimeOutChange?.Invoke(IsTimeOut, MaxTime);
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            IsRunning = false;
            OnTimeOutChange?.Invoke(IsTimeOut, MaxTime);
        }

        public void Resume()
        {
            IsRunning = true;
            OnTimeOutChange?.Invoke(IsTimeOut, MaxTime);
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
                    OnTimeOutChange?.Invoke(IsTimeOut, MaxTime);
                }
            }
        }

        public void Dispose()
        {
            OnTimeOutChange = null;
        }

        public void SetMaxTime(float time)
        {
            MaxTime = time;
            OnTimeOutChange?.Invoke(IsTimeOut, MaxTime);
        }
    }
}