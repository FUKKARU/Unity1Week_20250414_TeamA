using UnityEngine;

namespace NInGame
{
    public sealed class Sentence : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        [SerializeField, Range(0.0f, 10.0f), Tooltip("移動速度 (左方向が正)")] private float speed;
        [SerializeField, Range(-30.0f, 30.0f), Tooltip("ループするとき、どこに戻すか")] private float startX;
        [SerializeField, Range(-30.0f, 30.0f), Tooltip("ループするとき、どこで戻すか")] private float endX;

        private void Update()
        {
            rectTransform.localPosition += Vector3.left * (speed * Time.deltaTime);
            if (rectTransform.localPosition.x < endX)
            {
                Vector3 pos = rectTransform.localPosition;
                pos.x = startX;
                rectTransform.localPosition = pos;
            }
        }
    }
}