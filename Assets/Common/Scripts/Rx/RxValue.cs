using System;
using UnityEngine;

namespace Common.Scripts.Rxs
{
    public class RxValue<T>
    {
        public RxValue(T value)
        {
            _value = value;    
        }
        
        private T _value;
        public event Action<T> OnChange;

        public T Value
        {
            get => _value; 
            set
            {
                if (_value.Equals(value))
                {
                    return;
                }
                else
                {
                    _value = value;
                    OnChange?.Invoke(value);
                }
            }
        }
    }
}
