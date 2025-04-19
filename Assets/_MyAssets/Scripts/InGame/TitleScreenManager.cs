using UnityEngine;
using UnityEngine.UI;
using NGeneral;

namespace NInGame
{
    public sealed class TitleScreenManager : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        private void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(() => Scene.StageSelect.Load());
        }
    }
}