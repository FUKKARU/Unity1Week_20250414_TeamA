using UnityEngine;
using State = NInGame.CharacterState;
using Param = NInGame.CharacterParameters;
using System.Collections;

namespace NInGame
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField] protected new Transform transform;
        [SerializeField] protected new Rigidbody2D rigidbody;

        public State NowState { get; set; } = State.Stop;
        private State preState = State.Stop;

        protected virtual Vector3 Forward => transform.right;
        protected virtual void OnDied() { }

        private bool hasDied = false;
        private Coroutine jumpCoroutine = null;

        private void Update()
        {
            if (hasDied) return;

            if (transform.position.y < Param.KillY)
            {
                hasDied = true;
                OnDied();
                return;
            }

            if (NowState != preState)
            {
                // Stateが変化した

                StopMove();
                StopJump();

                switch (NowState)
                {
                    case State.Walk:
                        Move(Param.WalkVelocity);
                        break;
                    case State.Run:
                        Move(Param.RunVelocity);
                        break;

                    case State.Jump:
                        Jump(Param.JumpForce);
                        break;
                    default:
                        break;
                }
            }
            preState = NowState;

            AttitudeControl();
        }

        private void Move(float velocity)
        {
            if (rigidbody != null)
                rigidbody.linearVelocity = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * Forward * velocity;
        }

        private void Jump(float force)
        {
            if (rigidbody == null) return;

            if (jumpCoroutine != null) return;
            jumpCoroutine = StartCoroutine(JumpCoroutine(force));

            IEnumerator JumpCoroutine(float force)
            {
                while (true)
                {
                    rigidbody.AddForce(transform.up * force, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(Param.JumpInterval);
                }
            }
        }

        // 地面と垂直に姿勢制御
        private void AttitudeControl()
        {
            Vector2 footPos = (Vector2)transform.position - (Vector2)transform.up * (transform.lossyScale.y * 0.5f);
            RaycastHit2D[] hits = Physics2D.RaycastAll(footPos, Vector2.down, 1.0f, LayerMask.GetMask("stage/ground"));
            if (hits.Length > 0)
            {
                RaycastHit2D firstHit = hits[0];
                Vector2 normal = firstHit.normal;
                float angle = Vector2.SignedAngle(Vector2.up, normal);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        // 進行方向の速度をゼロにする
        private void StopMove()
        {
            if (rigidbody != null)
            {
                float angle = transform.eulerAngles.z;
                float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
                float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
                float cos2 = Mathf.Cos(angle * 2 * Mathf.Deg2Rad);

                float vx = rigidbody.linearVelocity.x;
                float vy = rigidbody.linearVelocity.y;

                float coef = (-vx * sin + vy * cos) / cos2;
                Vector3 velocity = coef * new Vector2(sin, cos);

                rigidbody.linearVelocity = velocity;
            }
        }

        private void StopJump()
        {
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }
        }
    }
}