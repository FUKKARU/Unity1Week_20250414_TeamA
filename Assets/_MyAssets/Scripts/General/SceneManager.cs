using UnityEngine;
using UniSceneManager = UnityEngine.SceneManagement.SceneManager;

namespace NGeneral
{
    public enum Scene
    {
        Title,
        StageSelect,
        Stage1,
    }

    public static class SceneManager
    {
        /// <summary>
        /// null なら、現在のシーンをロードし直す
        /// </summary>
        public static void Load(this Scene? scene)
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            string sceneName = scene?.ToSceneName() ?? string.Empty;
            if (string.IsNullOrEmpty(sceneName))
                sceneName = UniSceneManager.GetActiveScene().name;

            UniSceneManager.LoadScene(sceneName);
        }

        private static string ToSceneName(this Scene scene) => scene switch
        {
            Scene.Title => "Title",
            Scene.StageSelect => "StageSelect",
            Scene.Stage1 => "Stage1",
            _ => string.Empty,
        };
    }
}