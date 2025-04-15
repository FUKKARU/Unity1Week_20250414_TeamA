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

        public State State { get; set; } = State.Stop;

        protected abstract Vector3 Forward { get; }

        private Coroutine jumpCoroutine = null;
        private bool hasStopped = true;

        private void Update()
        {
            switch (State)
            {
                case State.Walk:
                    {
                        hasStopped = false;
                        StopJump();
                        Move(Param.WalkVelocity);
                    }
                    break;
                case State.Run:
                    {
                        hasStopped = false;
                        StopJump();
                        Move(Param.RunVelocity);
                    }
                    break;
                case State.Stop:
                    {
                        StopJump();
                        Stop();
                    }
                    break;
                case State.Jump:
                    {
                        hasStopped = false;
                        Jump(Param.JumpForce);
                    }
                    break;
                default:
                    break;
            }
        }

        private void Move(float velocity)
        {
            if (rigidbody != null)
                rigidbody.linearVelocity = Forward * velocity;

            // 地面と垂直に姿勢制御
            Vector3 footPos = transform.position - Vector3.up * (transform.lossyScale.y * 0.5f);
            RaycastHit2D hit = Physics2D.Raycast(footPos, Vector2.down, 0.05f, LayerMask.GetMask("stage/ground"));
            if (hit)
            {
                Vector2 normal = hit.normal;
                float angle = Vector2.SignedAngle(Vector2.up, normal);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
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

        private void StopJump()
        {
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }
        }

        private void Stop()
        {
            if (hasStopped) return;
            hasStopped = true;

            if (rigidbody != null)
                rigidbody.linearVelocity = Vector2.zero;
        }
    }
}