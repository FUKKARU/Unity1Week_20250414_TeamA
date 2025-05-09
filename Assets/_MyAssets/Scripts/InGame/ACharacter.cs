using System.Collections;
using UnityEngine;
using NScriptableObject;
using State = NInGame.CharacterState;

namespace NInGame
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField] protected new Transform transform;
        [SerializeField] protected new Collider2D collider;
        [SerializeField] protected new Rigidbody2D rigidbody;
        [SerializeField] protected Animator animator;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        private SParam.CCharacter param => SParam.Entity.Character;

        private static readonly int DiedHash = Animator.StringToHash("Died");
        private static readonly int WalkLeftHash = Animator.StringToHash("WalkLeft");
        private static readonly int WalkRightHash = Animator.StringToHash("WalkRight");
        private static readonly int RunLeftHash = Animator.StringToHash("RunLeft");
        private static readonly int RunRightHash = Animator.StringToHash("RunRight");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int IdleHash = Animator.StringToHash("Idle");

        public State NowState { get; set; } = State.Stop;
        private State preState = State.Stop;

        protected virtual Vector3 Forward => transform.right;
        protected virtual void OnDied()
        {
            AudioManager.Instance.DoPlay(SSound.Entity.SE.Died, AudioManager.AudioType.SE);
            if (animator != null)
                animator.SetTrigger(DiedHash);
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            var col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            StartCoroutine(DisableSelf());

            IEnumerator DisableSelf()
            {
                // Animatorの"Die"アニメの長さに応じて遅らせる（例：1秒）
                yield return new WaitForSecondsRealtime(1.0f);

                if (collider != null)
                    collider.enabled = false;
                if (rigidbody != null)
                    rigidbody.simulated = false;
                if (animator != null)
                    animator.enabled = false;
                if (spriteRenderer != null)
                    spriteRenderer.enabled = false;
            }
        }

        private bool hasDied = false;
        private Coroutine jumpCoroutine = null;

        private void Update()
        {
            if (hasDied) return;

            if (transform.position.y < param.KillY)
            {
                hasDied = true;
                if (animator != null)
                    animator.SetTrigger(DiedHash);
                OnDied();
                return;
            }

            if (NowState != preState)
            {
                // Stateが変化した

                StopMove();
                StopJump();
                SetBoolsToAnimator(NowState);

                switch (NowState)
                {
                    case State.WalkLeft:
                        Move(-param.WalkVelocity);
                        SetBoolsToAnimator(State.WalkLeft);
                        break;
                    case State.WalkRight:
                        Move(param.WalkVelocity);
                        SetBoolsToAnimator(State.WalkRight);
                        break;
                    case State.RunLeft:
                        Move(-param.RunVelocity);
                        SetBoolsToAnimator(State.RunLeft);
                        break;
                    case State.RunRight:
                        Move(param.RunVelocity);
                        SetBoolsToAnimator(State.RunRight);
                        break;
                    case State.Jump:
                        Jump(param.JumpForce);
                        SetBoolsToAnimator(State.Jump);
                        break;
                    case State.Stop:
                        SetBoolsToAnimator(State.Stop);
                        break;
                    default:
                        break;
                }
            }
            preState = NowState;

            // 跳ね返された時などに、反対方向に進んで行かないようにする
            switch (NowState)
            {
                case State.Stop:
                    if (rigidbody != null && rigidbody.linearVelocity != Vector2.zero)
                        StopMove();
                    break;
                case State.WalkLeft or State.RunLeft:
                    if (rigidbody != null && rigidbody.linearVelocity.x > 0)
                        StopMove();
                    break;
                case State.WalkRight or State.RunRight:
                    if (rigidbody != null && rigidbody.linearVelocity.x < 0)
                        StopMove();
                    break;
                case State.Jump:
                    if (rigidbody != null && rigidbody.linearVelocity.x != 0)
                        StopMove();
                    break;
                default:
                    break;
            }

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
                    yield return new WaitForSeconds(param.JumpInterval);
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

        private void SetBoolsToAnimator(State state)
        {
            if (animator == null) return;

            // すべてのboolをfalseに
            animator.SetBool(WalkLeftHash, false);
            animator.SetBool(WalkRightHash, false);
            animator.SetBool(RunLeftHash, false);
            animator.SetBool(RunRightHash, false);
            animator.SetBool(IdleHash, false);
            animator.SetBool(JumpHash, false);

            // 対応するステートだけtrueまたはtrigger
            switch (state)
            {
                case State.WalkLeft:
                    animator.SetBool(WalkLeftHash, true);
                    break;
                case State.WalkRight:
                    animator.SetBool(WalkRightHash, true);
                    break;
                case State.RunLeft:
                    animator.SetBool(RunLeftHash, true);
                    break;
                case State.RunRight:
                    animator.SetBool(RunRightHash, true);
                    break;
                case State.Jump:
                    animator.SetBool(JumpHash, true);
                    break;
                case State.Stop:
                    animator.SetBool(IdleHash, true); // IdleはboolとしてON
                    break;
            }
        }
    }
}