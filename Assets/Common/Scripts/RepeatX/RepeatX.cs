using System;
using UnityEngine;

namespace Common.Scripts.RepeatX
{
    public static class RepeatX
    {
        public static void Times(int count, Action<int> action)
        {
            for (int i = 0; i < count; i++) action(i);
        }
    }
}
