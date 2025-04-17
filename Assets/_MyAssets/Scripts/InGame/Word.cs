using System;
using NGeneral;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NInGame
{
    public sealed class Word : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
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
            FollowMouse();
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

            Debug.Log($"putPosition: {putPosition}, state: {state}, existed: {existed}, putSentence: {putSentence}");

            if (putPosition.HasValue) // はめ込める
            {
                this.putPosition = putPosition.Value.SetZ(0);
                transform.localPosition = this.putPosition;
                putState = state;
                if (putSentence != null) // Sentence と一緒に動くようにする
                {
                    if (putSentence.FollowerWord == default)
                        putSentence.FollowerWord = (this, OnSelfWasMoved);
                    nowSentence = putSentence;
                }
                OnPut?.Invoke(putState);
            }
            else if (existed) // はめ込んであるところに、はめ込もうとした
            {
                transform.localPosition = this.putPosition;
            }
            else if (putState != CharacterState.None) // はめ込んでいる状態から、外す
            {
                ForciblyPutOut();
            }
            else // 何もないところに、はめ込もうとした
            {
                transform.localPosition = this.putPosition;
            }
        }

        private void FollowMouse()
        {
            if (!isFollowing) return;

            Vector3 mousePosition = Input.mousePosition.SetZ(0);
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);

            // ウィンドウ内に制限
            if (canvas != null)
            {
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                float canvasWidth = canvasRect.rect.width;
                float canvasHeight = canvasRect.rect.height;
                float canvasWidthHalf = canvasWidth * 0.45f; // 少し小さめに
                float canvasHeightHalf = canvasHeight * 0.45f; // 少し小さめに

                pos.x = Mathf.Clamp(pos.x, -canvasWidthHalf, canvasWidthHalf);
                pos.y = Mathf.Clamp(pos.y, -canvasHeightHalf, canvasHeightHalf);
            }

            transform.localPosition = pos;
        }

        private void OnSelfWasMoved(Vector3 pos)
        {
            // putPosition を更新
            if (putState != CharacterState.None)
                putPosition = pos;
        }

        // 強制的に外す
        public void ForciblyPutOut()
        {
            this.putPosition = initPosition.SetZ(0);
            if (nowSentence != null) // これまでの Sentence と一緒に動いていたので、外す
            {
                if (nowSentence.FollowerWord != default)
                    nowSentence.FollowerWord = default;
                nowSentence = null;
            }
            transform.localPosition = this.putPosition;
            putState = CharacterState.Stop;
            OnPut?.Invoke(putState);
        }
    }
}