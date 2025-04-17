using System;
using NGeneral;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NInGame
{
    public sealed class Word : MonoBehaviour
    {
        [SerializeField, Tooltip("Z座標を算出するときに使用")] private Canvas canvas;
        [SerializeField] private Transform parent;
        [SerializeField] private new Transform transform;
        [SerializeField] private EventTrigger eventTrigger;

        private bool isFollowing = false;
        private Vector3 initPosition; // 最初に置かれていた座標
        private Vector3 putPosition; // 現在、ドラッグを話したときに、どこの座標に持っていくべきか
        private Sentence nowSentence = null; // 現在、はめ込まれているSentence

        // どこに嵌め込まれているかを保存
        private CharacterState putState = CharacterState.None;

        public Vector2 Position => transform.localPosition;

        // 単語をはめ込めるか調べる
        public Func<Transform, (Vector3?, CharacterState, bool, Sentence)> CheckPutOnPointerUp { get; set; } = null;

        // はめ込んだ際に実行する
        public Action<CharacterState> OnPut { get; set; } = null;

        private void Start()
        {
            initPosition = transform.localPosition;
            putPosition = initPosition;

            if (eventTrigger != null)
            {
                eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
                eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
            }
        }

        private void Update()
        {
            if (isFollowing)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = -0.1f;
                transform.localPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            }
        }

        private void OnPointerDown()
        {
            isFollowing = true;

            // つかんだ瞬間、最後の子にする
            if (transform != null && parent != null)
            {
                transform.SetParent(parent);
                transform.SetAsLastSibling();
            }
        }

        private void OnPointerUp()
        {
            isFollowing = false;

            (Vector3? putPosition, CharacterState state, bool existed, Sentence putSentence) = CheckPutOnPointerUp?.Invoke(transform) ?? default;

            if (putPosition.HasValue) // はめ込める
            {
                this.putPosition = putPosition.Value.SetZ(0);
                transform.localPosition = this.putPosition;
                putState = state;
                if (putSentence != null) // Sentence と一緒に動くようにする
                {
                    if (putSentence.FollowerWords.Exists(x => x.target == transform))
                        putSentence.FollowerWords.Remove((transform, OnSelfWasMoved));
                    putSentence.FollowerWords.Add((transform, OnSelfWasMoved));
                    nowSentence = putSentence;
                }
                OnPut?.Invoke(putState);
            }
            else if (existed) // はめ込んであるところに、はめ込もうとした
            {
                Debug.Log(nowSentence == null ? "null" : nowSentence.name);
                transform.localPosition = this.putPosition;
            }
            else if (putState != CharacterState.None) // はめ込んでいる状態から、外す
            {
                this.putPosition = initPosition.SetZ(0);
                transform.localPosition = this.putPosition;
                putState = CharacterState.Stop;
                if (nowSentence != null) // これまでの Sentence と一緒に動いていたので、外す
                {
                    if (nowSentence.FollowerWords.Exists(x => x.target == transform))
                        nowSentence.FollowerWords.Remove((transform, OnSelfWasMoved));
                    nowSentence = null;
                }
                OnPut?.Invoke(putState);
            }
            else // 何もないところに、はめ込もうとした
            {
                transform.localPosition = this.putPosition;
            }
        }

        private void OnSelfWasMoved(Vector3 pos)
        {
            // putPosition を更新
            if (putState != CharacterState.None)
                putPosition = pos;
        }
    }
}