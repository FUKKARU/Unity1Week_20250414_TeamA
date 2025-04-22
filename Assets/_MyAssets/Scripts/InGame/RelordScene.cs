using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneButton : MonoBehaviour
{
    public void ReloadScene()
    {
        // 現在のシーンをリロード
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
