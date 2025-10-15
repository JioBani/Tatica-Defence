using System;
using UnityEngine;

namespace Scenes.Battle.Scripts.Unit.Aggressor
{
    public class Aggressor : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2d;
        
        private void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _rigidbody2d.linearVelocity = Vector2.down * 1f;
        }

        private void OnDisable()
        {
            _rigidbody2d.linearVelocity = Vector2.zero;
        }
    }
}
