using IndianSubwayRunner.Core;
using IndianSubwayRunner.GameFlow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IndianSubwayRunner.Editor
{
    public static class SceneBootstrapGenerator
    {
        private const string SceneDirectory = "Assets/Scenes";

        [MenuItem("Indian Subway Runner/Generate Game Scenes")]
        public static void GenerateAll()
        {
            EnsureFolder("Assets", "Scenes");

            BuildScene("Boot", GameStartMode.Boot);
            BuildScene("MainMenu", GameStartMode.MainMenu);
            BuildScene("StoryIntro", GameStartMode.StoryIntro);
            BuildScene("Game", GameStartMode.Game);
            BuildScene("Results", GameStartMode.Results);

            EditorBuildSettings.scenes = new[]
            {
                Scene("Boot"),
                Scene("MainMenu"),
                Scene("StoryIntro"),
                Scene("Game"),
                Scene("Results")
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Indian Subway Runner scenes generated and added to build settings.");
        }

        private static EditorBuildSettingsScene Scene(string name)
        {
            return new EditorBuildSettingsScene(ScenePath(name), true);
        }

        private static string ScenePath(string name)
        {
            return SceneDirectory + "/" + name + ".unity";
        }

        private static void BuildScene(string name, GameStartMode mode)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject root = new GameObject("GameRoot");
            GameBootstrap bootstrap = root.AddComponent<GameBootstrap>();
            bootstrap.InitialMode = mode;
            EditorSceneManager.SaveScene(scene, ScenePath(name));
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            AssetDatabase.CreateFolder(parent, child);
        }
    }
}
