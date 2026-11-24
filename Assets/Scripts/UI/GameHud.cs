using IndianSubwayRunner.Core;
using IndianSubwayRunner.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IndianSubwayRunner.UI
{
    public sealed class GameHud : MonoBehaviour
    {
        private RunnerSimulation _simulation;
        private GameStartMode _mode = GameStartMode.Game;
        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _buttonStyle;
        private string _message = string.Empty;
        private float _messageUntil;

        private void Awake()
        {
            _simulation = GetComponent<RunnerSimulation>();
            if (_simulation == null)
            {
                _simulation = FindFirstObjectByType<RunnerSimulation>();
            }

            if (_simulation != null)
            {
                _simulation.DialogueRequested += OnDialogueRequested;
                _simulation.RunStarted += OnRunStarted;
            }
        }

        private void OnDestroy()
        {
            if (_simulation == null)
            {
                return;
            }

            _simulation.DialogueRequested -= OnDialogueRequested;
            _simulation.RunStarted -= OnRunStarted;
        }

        public void SetMode(GameStartMode mode)
        {
            _mode = mode;
        }

        private void Update()
        {
            if (_messageUntil > 0f && Time.unscaledTime >= _messageUntil)
            {
                _message = string.Empty;
                _messageUntil = 0f;
            }

            if (KeyboardEscapePressed())
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private void OnGUI()
        {
            EnsureStyles();

            switch (_mode)
            {
                case GameStartMode.MainMenu:
                    DrawMainMenu();
                    break;
                case GameStartMode.StoryIntro:
                    DrawStoryIntro();
                    break;
                case GameStartMode.Results:
                    DrawResults();
                    break;
                case GameStartMode.Boot:
                    DrawBoot();
                    break;
                default:
                    DrawGameHud();
                    break;
            }
        }

        private void DrawGameHud()
        {
            if (_simulation == null)
            {
                return;
            }

            float width = Screen.width;
            float height = Screen.height;
            GUI.Label(new Rect(24f, 20f, 260f, 40f), "SCORE " + _simulation.Score.Score, _bodyStyle);
            GUI.Label(new Rect(24f, 60f, 260f, 40f), "COINS " + _simulation.State.Coins, _bodyStyle);
            GUI.Label(new Rect(24f, 100f, 260f, 40f), "DIST " + _simulation.State.Distance.ToString("F0") + " m", _bodyStyle);
            GUI.Label(new Rect(width - 284f, 20f, 260f, 40f), "CHASE " + _simulation.Chase.Current.ToString("F1") + " m", _bodyStyle);
            GUI.Label(new Rect(width - 284f, 60f, 260f, 40f), "SPEED " + _simulation.State.Speed.ToString("F1"), _bodyStyle);

            string powerUps = string.Empty;
            if (_simulation.PowerUps.IsMagnetActive)
            {
                powerUps += "MAGNET ";
            }

            if (_simulation.PowerUps.IsMultiplierActive)
            {
                powerUps += "2X ";
            }

            if (_simulation.PowerUps.HasShield)
            {
                powerUps += "SHIELD";
            }

            if (!string.IsNullOrEmpty(powerUps))
            {
                GUI.Label(new Rect(24f, 140f, 360f, 40f), powerUps, _bodyStyle);
            }

            if (!string.IsNullOrEmpty(_message))
            {
                GUI.Label(new Rect(width * 0.08f, height * 0.66f, width * 0.84f, 100f), _message, _bodyStyle);
            }

            if (_simulation.State.Phase == RunPhase.Ready && GUI.Button(CenterButton(0f, "START RUN"), "START RUN", _buttonStyle))
            {
                _simulation.BeginRun();
            }

            if (_simulation.State.Phase == RunPhase.Ended && GUI.Button(CenterButton(0f, "RESTART"), "RESTART", _buttonStyle))
            {
                _simulation.RestartRun();
            }
        }

        private void DrawMainMenu()
        {
            float width = Screen.width;
            float height = Screen.height;
            GUI.Label(new Rect(width * 0.05f, height * 0.2f, width * 0.9f, 80f), "INDIAN SUBWAY RUNNER", _titleStyle);
            GUI.Label(new Rect(width * 0.08f, height * 0.34f, width * 0.84f, 120f), "Offline 3D endless chase. Dodge trains, collect coins, escape the pursuer.", _bodyStyle);
            if (GUI.Button(CenterButton(0f, "PLAY"), "PLAY", _buttonStyle))
            {
                SceneManager.LoadScene("StoryIntro");
            }
        }

        private void DrawStoryIntro()
        {
            float width = Screen.width;
            float height = Screen.height;
            GUI.Label(new Rect(width * 0.05f, height * 0.14f, width * 0.9f, 80f), "THE GREAT RAILWAY ESCAPE", _titleStyle);
            GUI.Label(new Rect(width * 0.08f, height * 0.3f, width * 0.84f, 220f), "ACP Pradyuman has spotted the criminal at the station. Jump the barriers, slide under signals, and stay ahead of the express.", _bodyStyle);
            if (GUI.Button(CenterButton(0f, "RUN"), "RUN", _buttonStyle))
            {
                SceneManager.LoadScene("Game");
            }
        }

        private void DrawResults()
        {
            float width = Screen.width;
            float height = Screen.height;
            int score = _simulation == null ? 0 : _simulation.Score.Score;
            GUI.Label(new Rect(width * 0.05f, height * 0.2f, width * 0.9f, 80f), "ESCAPED", _titleStyle);
            GUI.Label(new Rect(width * 0.08f, height * 0.34f, width * 0.84f, 80f), "FINAL SCORE " + score, _bodyStyle);
            if (GUI.Button(CenterButton(-90f, "RUN AGAIN"), "RUN AGAIN", _buttonStyle))
            {
                SceneManager.LoadScene("Game");
            }

            if (GUI.Button(CenterButton(90f, "MENU"), "MENU", _buttonStyle))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private void DrawBoot()
        {
            if (GUI.Button(CenterButton(0f, "ENTER"), "ENTER", _buttonStyle))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private Rect CenterButton(float yOffset, string label)
        {
            float width = Mathf.Min(420f, Screen.width * 0.72f);
            float height = 64f;
            return new Rect(
                (Screen.width - width) * 0.5f,
                Screen.height * 0.5f + yOffset - height * 0.5f,
                width,
                height);
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(28, Mathf.RoundToInt(Screen.height * 0.055f)),
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.84f, 0.2f) }
            };
            _bodyStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = Mathf.Max(18, Mathf.RoundToInt(Screen.height * 0.026f)),
                normal = { textColor = Color.white }
            };
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.Max(20, Mathf.RoundToInt(Screen.height * 0.03f)),
                fontStyle = FontStyle.Bold
            };
        }

        private void OnDialogueRequested(string line)
        {
            _message = line;
            _messageUntil = Time.unscaledTime + 4f;
        }

        private void OnRunStarted()
        {
            _message = "Run! Swipe left/right, up to jump, down to slide.";
            _messageUntil = Time.unscaledTime + 3.5f;
        }

        private static bool KeyboardEscapePressed()
        {
            return UnityEngine.InputSystem.Keyboard.current != null &&
                   UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
        }
    }
}
