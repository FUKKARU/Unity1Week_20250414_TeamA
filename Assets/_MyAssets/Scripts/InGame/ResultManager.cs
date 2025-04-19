using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Text = TMPro.TextMeshProUGUI;
using NGeneral;
using System.Collections;

namespace NInGame
{
    public sealed class ResultManager : MonoBehaviour
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private Text text;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Text retryButtonText;
        [SerializeField] private Text backButtonText;
        [SerializeField] private Image frontBlockingImage;

        private bool isShowing = false;
        private bool hasButtonClicked = false;

        private void Start()
        {
            if (parent != null)
            {
                parent.gameObject.SetActive(false);
                parent.anchoredPosition = new Vector2(0, 1080);
            }
        }

        public void Show(bool cleared)
        {
            if (isShowing) return;
            isShowing = true;

            StartCoroutine(ShowCoroutine(cleared));
        }

        private IEnumerator ShowCoroutine(bool cleared)
        {
            // 1秒待ってから Time.timeScale を止める（アニメーション再生のため）
            yield return new WaitForSeconds(1f);

            Time.timeScale = 0f;

            // 以下は元の Show メソッドの処理と同じ
            if (cleared)
            {
                if (text != null)
                    text.text = "I CLEARED the stage!!!";
                if (retryButton != null)
                    retryButtonText.text = "I go to NEXT";
                if (backButton != null)
                    backButtonText.text = "I'll be BACK";

                if (retryButton != null)
                    retryButton.onClick.AddListener(() => OnLoadSceneButtonClicked(SceneManager.NextScene()));
                if (backButton != null)
                    backButton.onClick.AddListener(() => OnLoadSceneButtonClicked(Scene.StageSelect));
            }
            else
            {
                if (text != null)
                    text.text = "I FAILED...";
                if (retryButton != null)
                    retryButtonText.text = "I wanna RETRY";
                if (backButton != null)
                    backButtonText.text = "I'll be BACK";

                if (retryButton != null)
                    retryButton.onClick.AddListener(() => OnLoadSceneButtonClicked(null));
                if (backButton != null)
                    backButton.onClick.AddListener(() => OnLoadSceneButtonClicked(Scene.StageSelect));
            }

            StartCoroutine(DoResultTween());
        }

        private IEnumerator DoResultTween()
        {
            yield return new WaitForSecondsRealtime(CharacterParameters.ResultIntervalOnCleared);

            if (parent == null) yield break;

            parent.gameObject.SetActive(true);
            parent.DOAnchorPosY(0, 0.5f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (frontBlockingImage != null)
                        frontBlockingImage.gameObject.SetActive(false);
                });
        }

        private void OnLoadSceneButtonClicked(Scene? scene)
        {
            if (hasButtonClicked) return;
            hasButtonClicked = true;

            scene.Load();
        }
    }
}