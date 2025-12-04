using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Scripts.RepeatX
{
    public static class RepeatX
    {
        public static void Times(int count, Action<int> action)
        {
            for (int i = 0; i < count; i++) action(i);
        }

        public static List<T> Times<T>(int count, Func<int, T> func)
        {
            var results = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                results.Add(func(i));
            }

            return results;
        }
    }
}
