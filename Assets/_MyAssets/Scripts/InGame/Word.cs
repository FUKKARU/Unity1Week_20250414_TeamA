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
        private Vector3 initPosition;
        private Vector3 origin;

        // どこに嵌め込まれているかを保存
        private CharacterState putState = CharacterState.None;

        public Vector2 Position => transform.localPosition;

        // 単語をはめ込めるか調べ、はめ込めるならその座標を、はめ込めないならnullを返す
        // はめ込んだものの種類も返す
        public Func<Transform, (Vector3?, CharacterState, bool)> CheckPutOnPointerUp { get; set; } = null;

        // はめ込んだ際に実行する
        public Action<CharacterState> OnPut { get; set; } = null;

        private void Start()
        {
            initPosition = transform.localPosition;
            origin = transform.localPosition;

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

            (Vector3? putPosition, CharacterState state, bool existed) = CheckPutOnPointerUp?.Invoke(transform) ?? default;

            if (putPosition.HasValue) // はめ込める
            {
                origin = putPosition.Value.SetZ(0);
                transform.localPosition = origin;
                putState = state;
                OnPut?.Invoke(putState);
            }
            else if (existed) // はめ込んであるところに、はめ込もうとした
            {
                transform.localPosition = origin;
            }
            else if (putState != CharacterState.None) // はめ込んでいる状態から、外す
            {
                origin = initPosition.SetZ(0);
                transform.localPosition = origin;
                putState = CharacterState.Stop;
                OnPut?.Invoke(putState);
            }
            else // 何もないところに、はめ込もうとした
            {
                transform.localPosition = origin;
            }
        }
    }
}