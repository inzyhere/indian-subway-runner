using IndianSubwayRunner.Core;
using IndianSubwayRunner.Gameplay;
using UnityEngine;

namespace IndianSubwayRunner.Audio
{
    public sealed class GameAudio : MonoBehaviour
    {
        private RunnerSimulation _simulation;
        private AudioSource _music;
        private AudioSource _effects;
        private AudioClip _musicLoop;
        private AudioClip _coin;
        private AudioClip _jump;
        private AudioClip _slide;
        private AudioClip _crash;
        private AudioClip _powerUp;
        private AudioClip _trainHorn;
        private AudioClip _uiClick;

        private void Awake()
        {
            _simulation = GetComponent<RunnerSimulation>();
            if (_simulation == null)
            {
                _simulation = FindFirstObjectByType<RunnerSimulation>();
            }

            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true;
            _music.playOnAwake = false;
            _music.volume = 0.35f;
            _effects = gameObject.AddComponent<AudioSource>();
            _effects.playOnAwake = false;
            _effects.volume = 0.8f;

            _musicLoop = Load("Audio/Music/ISR_Loop_Main_001");
            _coin = Load("Audio/SFX/ISR_SFX_Coin_001");
            _jump = Load("Audio/SFX/ISR_SFX_Jump_001");
            _slide = Load("Audio/SFX/ISR_SFX_Slide_001");
            _crash = Load("Audio/SFX/ISR_SFX_Crash_001");
            _powerUp = Load("Audio/SFX/ISR_SFX_PowerUp_001");
            _trainHorn = Load("Audio/SFX/ISR_SFX_TrainHorn_001");
            _uiClick = Load("Audio/SFX/ISR_SFX_UiClick_001");

            if (_simulation != null)
            {
                _simulation.ActionAccepted += OnActionAccepted;
                _simulation.ContactResolved += OnContactResolved;
                _simulation.RunStarted += OnRunStarted;
            }
        }

        private void Start()
        {
            if (_musicLoop == null)
            {
                return;
            }

            _music.clip = _musicLoop;
            _music.Play();
        }

        private void OnDestroy()
        {
            if (_simulation == null)
            {
                return;
            }

            _simulation.ActionAccepted -= OnActionAccepted;
            _simulation.ContactResolved -= OnContactResolved;
            _simulation.RunStarted -= OnRunStarted;
        }

        private void OnRunStarted()
        {
            Play(_uiClick);
        }

        private void OnActionAccepted(GameAction action)
        {
            if (action == GameAction.Jump)
            {
                Play(_jump);
            }
            else if (action == GameAction.Slide)
            {
                Play(_slide);
            }
        }

        private void OnContactResolved(CollisionOutcome outcome, CollisionContact contact)
        {
            switch (outcome)
            {
                case CollisionOutcome.CollectCoin:
                    Play(_coin);
                    break;
                case CollisionOutcome.CollectPowerUp:
                    Play(_powerUp);
                    break;
                case CollisionOutcome.ShieldAbsorbed:
                    Play(_powerUp);
                    break;
                case CollisionOutcome.Stumble:
                    Play(_trainHorn);
                    break;
                case CollisionOutcome.Crash:
                    Play(_crash);
                    break;
            }
        }

        private void Play(AudioClip clip)
        {
            if (clip != null)
            {
                _effects.PlayOneShot(clip);
            }
        }

        private static AudioClip Load(string path)
        {
            return Resources.Load<AudioClip>(path);
        }
    }
}
