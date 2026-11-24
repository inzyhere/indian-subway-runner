using IndianSubwayRunner.Core;
using IndianSubwayRunner.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IndianSubwayRunner.Player
{
    public sealed class SwipeInput : MonoBehaviour
    {
        [SerializeField] private float minimumSwipePixels = 48f;

        private RunnerSimulation _simulation;
        private Vector2 _touchStart;
        private bool _touchActive;

        private void Awake()
        {
            _simulation = GetComponent<RunnerSimulation>();
            if (_simulation == null)
            {
                _simulation = FindFirstObjectByType<RunnerSimulation>();
            }
        }

        private void Update()
        {
            if (_simulation == null || !_simulation.IsRunning)
            {
                return;
            }

            ReadKeyboard();
            ReadTouch();
        }

        private void ReadKeyboard()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                _simulation.Submit(GameAction.MoveLeft);
            }

            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                _simulation.Submit(GameAction.MoveRight);
            }

            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
            {
                _simulation.Submit(GameAction.Jump);
            }

            if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            {
                _simulation.Submit(GameAction.Slide);
            }
        }

        private void ReadTouch()
        {
            Touchscreen touchscreen = Touchscreen.current;
            if (touchscreen == null)
            {
                return;
            }

            TouchControl touch = touchscreen.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                _touchStart = touch.position.ReadValue();
                _touchActive = true;
            }

            if (!_touchActive || !touch.press.wasReleasedThisFrame)
            {
                return;
            }

            Vector2 delta = touch.position.ReadValue() - _touchStart;
            _touchActive = false;
            if (delta.magnitude < minimumSwipePixels)
            {
                return;
            }

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                _simulation.Submit(delta.x > 0f ? GameAction.MoveRight : GameAction.MoveLeft);
                return;
            }

            _simulation.Submit(delta.y > 0f ? GameAction.Jump : GameAction.Slide);
        }
    }
}
