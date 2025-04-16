using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // ゲームシーン名
    [SerializeField] private string gameSceneName = "StageSelect";

    // スタートボタンが押されたときに呼ばれる
    public void OnStartGameButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    //// 終了ボタンが押されたときに呼ばれる
    //public void OnQuitButton()
    ////{
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif
//    }
}
