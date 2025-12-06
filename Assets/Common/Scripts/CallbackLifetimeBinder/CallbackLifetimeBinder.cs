using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Scripts.CallbackLifetimeBinders
{
    public class CallbackLifetimeBinder : MonoBehaviour
    {
        private readonly List<(Action onEnable, Action onDisable)> _bindings
            = new List<(Action, Action)>();

        private bool _isBound = false;

        /// <summary>
        /// Awake 등에서 호출해서
        /// "이 오브젝트가 활성화될 때 할 일 / 비활성화될 때 할 일"을 등록한다.
        /// </summary>
        protected void Bind(Action onEnable, Action onDisable)
        {
            _bindings.Add((onEnable, onDisable));
        }

        private void OnEnable()
        {
            if (_isBound) return;

            foreach (var (onEnable, _) in _bindings)
            {
                onEnable?.Invoke();
            }

            _isBound = true;
        }

        private void OnDisable()
        {
            if (!_isBound) return;

            foreach (var (_, onDisable) in _bindings)
            {
                onDisable?.Invoke();
            }

            _isBound = false;
        }

        private void OnDestroy()
        {
            // 혹시 Enable 상태에서 Destroy 될 수도 있으니 마지막으로 한 번 더 해제
            if (_isBound)
            {
                foreach (var (_, onDisable) in _bindings)
                {
                    onDisable?.Invoke();
                }

                _isBound = false;
            }

            _bindings.Clear();
        }
    }
}