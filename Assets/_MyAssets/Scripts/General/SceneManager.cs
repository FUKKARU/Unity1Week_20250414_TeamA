using UnityEngine;
using UniSceneManager = UnityEngine.SceneManagement.SceneManager;

namespace NGeneral
{
    public enum Scene
    {
        Title,
        StageSelect,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6,
        Stage7,
        Stage8,
        Stage9,
        Stage10,
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

        /// <summary>
        /// 次シーンを取得 (最終ステージの場合は、タイトル)
        /// </summary>
        public static Scene NextScene() => UniSceneManager.GetActiveScene().name.ToSceneEnum() switch
        {
            Scene.Title => Scene.StageSelect,
            Scene.StageSelect => Scene.Stage1,
            Scene.Stage1 => Scene.Stage2,
            Scene.Stage2 => Scene.Stage3,
            Scene.Stage3 => Scene.Stage4,
            Scene.Stage4 => Scene.Stage5,
            Scene.Stage5 => Scene.Stage6,
            Scene.Stage6 => Scene.Stage7,
            Scene.Stage7 => Scene.Stage8,
            Scene.Stage8 => Scene.Stage9,
            Scene.Stage9 => Scene.Stage10,
            Scene.Stage10 => Scene.Title,
            _ => Scene.Title,
        };

        private static string ToSceneName(this Scene scene) => scene switch
        {
            Scene.Title => "Title",
            Scene.StageSelect => "StageSelect",
            Scene.Stage1 => "Stage1",
            Scene.Stage2 => "Stage2",
            Scene.Stage3 => "Stage3",
            Scene.Stage4 => "Stage4",
            Scene.Stage5 => "Stage5",
            Scene.Stage6 => "Stage6",
            Scene.Stage7 => "Stage7",
            Scene.Stage8 => "Stage8",
            Scene.Stage9 => "Stage9",
            Scene.Stage10 => "Stage10",
            _ => string.Empty,
        };

        private static Scene ToSceneEnum(this string sceneName)
        {
            return sceneName switch
            {
                "Title" => Scene.Title,
                "StageSelect" => Scene.StageSelect,
                "Stage1" => Scene.Stage1,
                "Stage2" => Scene.Stage2,
                "Stage3" => Scene.Stage3,
                "Stage4" => Scene.Stage4,
                "Stage5" => Scene.Stage5,
                "Stage6" => Scene.Stage6,
                "Stage7" => Scene.Stage7,
                "Stage8" => Scene.Stage8,
                "Stage9" => Scene.Stage9,
                "Stage10" => Scene.Stage10,
                _ => Scene.Title,
            };
        }
    }
}