using UnityEngine;
using UnityEngine.UI;
using NGeneral;
using NScriptableObject;

namespace NInGame
{
    public sealed class StageSelectManager : MonoBehaviour
    {
        [SerializeField] private Button button1;
        [SerializeField] private Button button2;
        [SerializeField] private Button button3;
        [SerializeField] private Button button4;
        [SerializeField] private Button button5;
        [SerializeField] private Button button6;
        [SerializeField] private Button button7;
        [SerializeField] private Button button8;

        private void Start()
        {
            ExecuteEvent(button1, Scene.Stage1);
            ExecuteEvent(button2, Scene.Stage2);
            ExecuteEvent(button3, Scene.Stage3);
            ExecuteEvent(button4, Scene.Stage4);
            ExecuteEvent(button5, Scene.Stage5);
            ExecuteEvent(button6, Scene.Stage6);
            ExecuteEvent(button7, Scene.Stage7);
            ExecuteEvent(button8, Scene.Stage8);
        }

        private void ExecuteEvent(Button button, Scene scene)
        {
            if (button != null)
            {
                button.onClick.AddListener(() => scene.Load());
                AudioManager.Instance.DoPlay(SSound.Entity.SE.Clicked, AudioManager.AudioType.SE);
            }
        }
    }
}