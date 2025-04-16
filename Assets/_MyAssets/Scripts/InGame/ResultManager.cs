using NGeneral;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

namespace NInGame
{
    public sealed class ResultManager : MonoBehaviour
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private Text text;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button backButton;

        private bool isShowing = false;
        private bool hasButtonClicked = false;

        private void Start()
        {
            if (parent != null)
                parent.gameObject.SetActive(false);

            if (retryButton != null)
                retryButton.onClick.AddListener(() => OnLoadSceneButtonClicked(null));

            if (backButton != null)
                backButton.onClick.AddListener(() => OnLoadSceneButtonClicked(Scene.StageSelect));
        }

        public void Show(bool cleared)
        {
            if (isShowing) return;
            isShowing = true;

            Time.timeScale = 0f;

            if (text != null) text.text = cleared ? "Game Clear !!!" : "Failed ...";
            if (parent != null) parent.gameObject.SetActive(true);
        }

        private void OnLoadSceneButtonClicked(Scene? scene)
        {
            if (hasButtonClicked) return;
            hasButtonClicked = true;

            scene.Load();
        }
    }
}