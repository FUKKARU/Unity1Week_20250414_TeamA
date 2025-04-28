using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NGeneral;

namespace NInGame
{
    public sealed class ReloadButtonManager : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Start() => StartCoroutine(BindEvent());

        private IEnumerator BindEvent()
        {
            yield return new WaitForSeconds(0.1f);

            if (button != null)
                button.onClick.AddListener(() => SceneManager.NowScene.Load());
        }
    }
}
