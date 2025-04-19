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

            // Diedアニメーションを再生（ACharacter側がやってる場合は省略可能）
            base.OnDied();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("character/enemy") ||
                collision.gameObject.tag.Contains("stage/spike"))
            {
                OnDied(); // ←これで確実に死亡アニメーションや物理停止が入る
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("stage/goal"))
            {
                AudioManager.Instance.DoPlay(SSound.Entity.SE.Cleared, AudioManager.AudioType.SE);
                if (animator != null)
                    animator.SetTrigger(ClearedHash);
                var rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }

                var col = GetComponent<Collider2D>();
                if (col != null)
                    col.enabled = false; 
                OnPlayerCleared?.Invoke();
            }
        }
    }
}