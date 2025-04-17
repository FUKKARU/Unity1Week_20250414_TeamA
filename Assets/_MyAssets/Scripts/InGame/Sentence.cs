using System;
using System.Collections.Generic;
using UnityEngine;

namespace NInGame
{
    public sealed class Sentence : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField, Tooltip("Wordを自分と一緒に動かす用")] private RectTransform panel;

        [SerializeField, Range(0.0f, 10.0f), Tooltip("移動速度 (左方向が正)")] private float speed;
        [SerializeField, Range(-30.0f, 30.0f), Tooltip("ループするとき、どこに戻すか")] private float startX;
        [SerializeField, Range(-30.0f, 30.0f), Tooltip("ループするとき、どこで戻すか")] private float endX;

        // 一緒に動かすWord (外部から登録)
        public List<(Transform target, Action<Vector3> onTargetWasMoved)> FollowerWords { get; set; } = new List<(Transform, Action<Vector3>)>();

        private void Update()
        {
            rectTransform.localPosition += Vector3.left * (speed * Time.deltaTime);
            if (rectTransform.localPosition.x < endX)
            {
                Vector3 pos = rectTransform.localPosition;
                pos.x = startX;
                rectTransform.localPosition = pos;
            }

            // Wordを一緒に動かす
            foreach (var (target, onTargetWasMoved) in FollowerWords)
            {
                if (target != null)
                {
                    Vector3 pos = target.position;
                    pos.x = panel.position.x;
                    pos.y = panel.position.y;
                    target.position = pos;

                    onTargetWasMoved?.Invoke(pos);
                }
            }
        }

        private void OnDestroy()
        {
            FollowerWords?.Clear();
        }
    }
}