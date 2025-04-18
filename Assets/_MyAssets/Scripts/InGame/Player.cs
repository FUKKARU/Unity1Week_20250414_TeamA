using System;
using UnityEngine;
using NScriptableObject;

namespace NInGame
{
    public sealed class Player : ACharacter
    {
        public Action OnPlayerFailed { get; set; } = null;
        public Action OnPlayerCleared { get; set; } = null;

        private static readonly int ClearedHash = Animator.StringToHash("Cleared");

        protected override void OnDied()
        {
            OnPlayerFailed?.Invoke();
            base.OnDied();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("character/enemy") ||
                collision.gameObject.tag.Contains("stage/spike"))
                OnPlayerFailed?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("stage/goal"))
            {
                AudioManager.Instance.DoPlay(SSound.Entity.SE.Cleared, AudioManager.AudioType.SE);
                if (animator != null)
                    animator.SetBool(ClearedHash, true);
                OnPlayerCleared?.Invoke();
            }
        }
    }
}