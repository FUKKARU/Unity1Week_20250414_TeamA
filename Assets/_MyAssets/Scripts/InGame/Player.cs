using System;
using UnityEngine;

namespace NInGame
{
    public sealed class Player : ACharacter
    {
        public Action OnPlayerFailed { get; set; } = null;
        public Action OnPlayerCleared { get; set; } = null;

        protected override Vector3 Forward => transform.right;

        protected override void OnDied() => OnPlayerFailed?.Invoke();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("character/enemy"))
                OnPlayerFailed?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("stage/goal"))
                OnPlayerCleared?.Invoke();
        }
    }
}