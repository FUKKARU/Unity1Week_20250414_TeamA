using UnityEngine;
using UnityEngine.UI;
using NGeneral;
using NScriptableObject;

namespace NInGame
{
    public sealed class TitleScreenManager : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        private void Start()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(() => Scene.StageSelect.Load());
                AudioManager.Instance.DoPlay(SSound.Entity.SE.Clicked, AudioManager.AudioType.SE);
            }

            AudioManager.Instance.DoPlay(SSound.Entity.BGM.Main, AudioManager.AudioType.BGM);
        }
    }
}