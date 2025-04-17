using System;
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
        [SerializeField, Range(-30.0f, 30.0f), Tooltip("Word を強制的に外す、境界のx座標")] private float wordOutX;

        // 一緒に動かすWord (外部から登録)
        public (Word target, Action<Vector3> onTargetWasMoved) FollowerWord { get; set; } = default;

        private void Update()
        {
            rectTransform.localPosition += Vector3.left * (speed * Time.deltaTime);

            // Word を強制的に外す
            bool doForciblyPutOut = false;
            if (rectTransform.localPosition.x < wordOutX)
            {
                doForciblyPutOut = true;

                if (FollowerWord.target != null)
                    FollowerWord.target.ForciblyPutOut();
            }

            // ループする
            if (rectTransform.localPosition.x < endX)
            {
                Vector3 pos = rectTransform.localPosition;
                pos.x = startX;
                rectTransform.localPosition = pos;
            }

            // Wordを一緒に動かす
            if (!doForciblyPutOut)
            {
                if (FollowerWord.target != null)
                {
                    Vector3 pos = FollowerWord.target.transform.position;
                    pos.x = panel.position.x;
                    pos.y = panel.position.y;
                    FollowerWord.target.transform.position = pos;

                    FollowerWord.onTargetWasMoved?.Invoke(pos);
                }
            }
        }
    }
}