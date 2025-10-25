using System;
using UnityEngine;

namespace Scenes.Battle.Feature.Units.UnitStats
{
    public class UnitStats<T>
    {
        private T _currentValue;
        public event Action<T> OnChange;

        public T CurrentValue
        {
            get => _currentValue; 
            set
            {
                if (_currentValue.Equals(value))
                {
                    return;
                }
                else
                {
                    _currentValue = value;
                    OnChange?.Invoke(value);
                }
            }
        }
        
        public T OriginalValue { get; private set; }

        public void SetInitValue(T value)
        {
            OriginalValue = value;
            CurrentValue = value;
        }
    }
}
