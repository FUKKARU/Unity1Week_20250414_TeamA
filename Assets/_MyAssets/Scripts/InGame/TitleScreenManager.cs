using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // �Q�[���V�[����
    [SerializeField] private string gameSceneName = "StageSelect";

    // �X�^�[�g�{�^���������ꂽ�Ƃ��ɌĂ΂��
    public void OnStartGameButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    //// �I���{�^���������ꂽ�Ƃ��ɌĂ΂��
    //public void OnQuitButton()
    ////{
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif
//    }
}
