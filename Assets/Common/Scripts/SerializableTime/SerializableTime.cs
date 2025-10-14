using System;
using UnityEngine;

namespace Common.Scripts.SerializableTime
{
    /// <summary>
    /// 인스펙터에서 시간(hh:mm:ss.fff)을 자연스럽게 입력하기 위한 래퍼
    /// 코드에서는 TotalSeconds/TimeSpan으로 편리하게 사용
    /// </summary>
    [Serializable]
    public struct SerializableTime
    {
        [Min(0)] public int hours;
        [Range(0, 59)] public int minutes;
        [Range(0f, 59.999f)] public float seconds;

        public double TotalSeconds => hours * 3600d + minutes * 60d + seconds;
        public double TotalMilliseconds => TotalSeconds * 1000d;
        public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(TotalSeconds);

        /// <summary>초→분, 분→시 승급 및 범위 보정</summary>
        public void Normalize()
        {
            if (hours < 0) hours = 0;
            if (minutes < 0) minutes = 0;
            if (seconds < 0f) seconds = 0f;

            if (seconds >= 60f)
            {
                int addMin = Mathf.FloorToInt(seconds / 60f);
                seconds -= addMin * 60f;
                minutes += addMin;
            }

            if (minutes >= 60)
            {
                int addHour = minutes / 60;
                minutes -= addHour * 60;
                hours += addHour;
            }
        }

        public override string ToString()
        {
            var ts = ToTimeSpan();
            return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        public static SerializableTime FromSeconds(double totalSeconds)
        {
            if (totalSeconds < 0) totalSeconds = 0;
            int h = (int)(totalSeconds / 3600d);
            totalSeconds -= h * 3600d;
            int m = (int)(totalSeconds / 60d);
            double s = totalSeconds - m * 60d;

            var d = new SerializableTime { hours = h, minutes = m, seconds = (float)s };
            d.Normalize();
            return d;
        }

        public static SerializableTime FromTimeSpan(TimeSpan ts) => FromSeconds(ts.TotalSeconds);

        // 편의용 암시적 변환
        public static implicit operator TimeSpan(SerializableTime d) => d.ToTimeSpan();
        public static implicit operator SerializableTime(TimeSpan ts) => FromTimeSpan(ts);
    }
}
