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

        // クリックされる前の、Sentence にバインドしていたかの情報を保存
        private bool bindedBeforePointerDown = false;

        // ゲーム開始時の、Canvasの座標を保存
        private Vector2 canvasPositionOnStart = Vector2.zero;

        private void Start()
        {
            initPosition = transform.localPosition;
            putPosition = initPosition;

            if (canvas != null)
                canvasPositionOnStart = canvas.transform.position;

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

            bindedBeforePointerDown = GetBindedAsFollower();
            SetBindedAsFollower(false);

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
                    nowSentence = putSentence;
                    SetBindedAsFollower(true);
                }
                OnPut?.Invoke(putState);
            }
            else if (existed) // はめ込んであるところに、はめ込もうとした
            {
                transform.localPosition = this.putPosition;
                SetBindedAsFollower(bindedBeforePointerDown); // つかんでいたときの状態に戻す
            }
            else if (putState != CharacterState.None) // はめ込んでいる状態から、外す
            {
                ForciblyPutOut();
            }
            else // 何もないところに、はめ込もうとした
            {
                transform.localPosition = this.putPosition;
                SetBindedAsFollower(bindedBeforePointerDown); // つかんでいたときの状態に戻す
            }
        }

        private void FollowMouse()
        {
            if (!isFollowing) return;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 screenPosition = Input.mousePosition;

            // スクリーン座標をキャンバスのローカル座標に変換
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, Camera.main, out Vector2 localPoint))
            {
                Vector3 pos = localPoint; // z座標は0でOK

                // ウィンドウ内に制限
                float canvasWidthHalf = canvasRect.rect.width * 0.45f; // 少し小さく
                float canvasHeightHalf = canvasRect.rect.height * 0.45f; // 少し小さく

                pos.x = Mathf.Clamp(pos.x, -canvasWidthHalf, canvasWidthHalf);
                pos.y = Mathf.Clamp(pos.y, -canvasHeightHalf, canvasHeightHalf);

                transform.localPosition = pos;
            }
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
            SetBindedAsFollower(false); // これまでの Sentence と一緒に動いていたので、外す
            nowSentence = null;
            transform.localPosition = this.putPosition;
            putState = CharacterState.Stop;
            OnPut?.Invoke(putState);
        }

        private bool GetBindedAsFollower()
        {
            if (nowSentence == null) return false;
            return nowSentence.FollowerWord.target == this;
        }

        private void SetBindedAsFollower(bool doBind)
        {
            if (nowSentence == null) return;

            if (doBind)
            {
                if (!GetBindedAsFollower())
                    nowSentence.FollowerWord = (this, OnSelfWasMoved);
            }
            else
            {
                if (GetBindedAsFollower())
                    nowSentence.FollowerWord = default;
            }
        }
    }
}