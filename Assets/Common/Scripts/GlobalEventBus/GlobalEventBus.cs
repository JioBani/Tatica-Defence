using System;
using System.Collections.Generic;

namespace Common.Scripts.GlobalEventBus
{
    public interface IGameEvent { }

    public static class GlobalEventBus
    {
        private static readonly Dictionary<Type, Delegate> Handlers = new();

        public static void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            var type = typeof(T);
            if (Handlers.TryGetValue(type, out var existing))
            {
                Handlers[type] = (Action<T>)existing + handler;
            }
            else
            {
                Handlers[type] = handler;
            }
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            var type = typeof(T);
            if (Handlers.TryGetValue(type, out var existing))
            {
                var current = (Action<T>)existing;
                current -= handler;

                if (current == null)
                    Handlers.Remove(type);
                else
                    Handlers[type] = current;
            }
        }

        public static void Publish<T>(T evt) where T : struct, IGameEvent
        {
            var type = typeof(T);
            if (Handlers.TryGetValue(type, out var existing))
            {
                ((Action<T>)existing)?.Invoke(evt);
            }
        }
    }
}