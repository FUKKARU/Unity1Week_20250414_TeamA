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
    }

    public static class SceneManager
    {
        public static void Load(this Scene scene)
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            UniSceneManager.LoadScene(scene.ToSceneName());
        }

        /// <summary>
        /// 現在シーンを取得
        /// </summary>
        public static Scene NowScene => UniSceneManager.GetActiveScene().name.ToSceneEnum();

        /// <summary>
        /// 次シーンを取得 (最終ステージの場合は、タイトル)
        /// </summary>
        public static Scene NextScene => NowScene switch
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
            Scene.Stage8 => Scene.Title,
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
            _ => "Title",
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
                _ => Scene.Title,
            };
        }
    }
}