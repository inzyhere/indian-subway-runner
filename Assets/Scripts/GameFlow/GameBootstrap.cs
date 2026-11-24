using IndianSubwayRunner.Audio;
using IndianSubwayRunner.Core;
using IndianSubwayRunner.Gameplay;
using IndianSubwayRunner.Player;
using IndianSubwayRunner.ProceduralGeneration;
using IndianSubwayRunner.UI;
using UnityEngine;

namespace IndianSubwayRunner.GameFlow
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameStartMode initialMode = GameStartMode.Game;

        public GameStartMode InitialMode
        {
            get => initialMode;
            set => initialMode = value;
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 1f / 60f;
            Screen.orientation = ScreenOrientation.Portrait;

            EnsureCamera();
            EnsureLight();

            RunnerSimulation simulation = GetComponent<RunnerSimulation>();
            if (simulation == null)
            {
                simulation = gameObject.AddComponent<RunnerSimulation>();
            }

            gameObject.AddComponent<SwipeInput>();
            gameObject.AddComponent<RunnerView>();
            gameObject.AddComponent<ChunkView>();
            gameObject.AddComponent<GameAudio>();

            GameHud hud = gameObject.AddComponent<GameHud>();
            hud.SetMode(initialMode);

            if (initialMode == GameStartMode.Game)
            {
                simulation.BeginRun();
            }
        }

        private static void EnsureCamera()
        {
            Camera camera = FindFirstObjectByType<Camera>();
            if (camera != null)
            {
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 6.5f, -9f);
            cameraObject.transform.rotation = Quaternion.Euler(12f, 0f, 0f);
            Camera main = cameraObject.GetComponent<Camera>();
            main.clearFlags = CameraClearFlags.SolidColor;
            main.backgroundColor = new Color(0.05f, 0.07f, 0.13f);
            main.fieldOfView = 62f;
        }

        private static void EnsureLight()
        {
            Light existing = FindFirstObjectByType<Light>();
            if (existing != null)
            {
                return;
            }

            GameObject lightObject = new GameObject("Directional Light", typeof(Light));
            lightObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.35f;
            light.shadows = LightShadows.Soft;
        }
    }
}
