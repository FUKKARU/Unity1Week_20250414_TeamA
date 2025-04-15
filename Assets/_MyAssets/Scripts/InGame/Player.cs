using UnityEngine;

namespace NInGame
{
    public sealed class Player : ACharacter
    {
        protected override Vector3 Forward => transform.right;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("character/enemy"))
            {
                Debug.Log("Died!");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("stage/goal"))
            {
                Debug.Log("Goal!");
            }
        }
    }
}