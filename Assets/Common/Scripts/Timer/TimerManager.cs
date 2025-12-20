using System;
using System.Collections.Generic;
using UnityEngine;
using Common.Scripts.SceneSingleton;

namespace Common.Scripts.Timers
{
    public class TimerManager : SceneSingleton<TimerManager>
    {
        private readonly List<Timer> _timers = new List<Timer>();

        public Timer Make(float maxTime, Action<bool, float> onTimeOutChange = null)
        {
            Timer timer = new Timer(maxTime);
            if (onTimeOutChange != null)
            {
                timer.OnTimeOutChange += onTimeOutChange;
            }
            
            _timers.Add(timer);

            return timer;
        }

        public void Delete(Timer timer)
        {
            timer.Dispose();
            _timers.Remove(timer);
        }

        private void Update()
        {
            foreach (var timer in _timers)
            {
                if (timer.IsRunning)
                {
                    timer.Update(Time.deltaTime);
                }
            }
        }
    }
}
