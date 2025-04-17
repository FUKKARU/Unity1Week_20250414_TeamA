using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    // ステージ1に移動
    public void LoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }
}