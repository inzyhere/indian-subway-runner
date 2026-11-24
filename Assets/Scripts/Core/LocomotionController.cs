using System;

namespace IndianSubwayRunner.Core
{
    public sealed class LocomotionController
    {
        private readonly GameConfig _config;
        private float _elapsed;

        public LocomotionController(GameConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public LocomotionState State { get; private set; }

        public float Progress
        {
            get
            {
                if (State == LocomotionState.Running)
                {
                    return 0f;
                }

                float duration = State == LocomotionState.Jumping ? _config.JumpDuration : _config.SlideDuration;
                return Math.Min(1f, _elapsed / duration);
            }
        }

        public float VerticalOffset
        {
            get
            {
                return State == LocomotionState.Jumping ? (float)Math.Sin(Progress * Math.PI) * 2.2f : 0f;
            }
        }

        public float HeightScale
        {
            get
            {
                return State == LocomotionState.Sliding ? 1f - (float)Math.Sin(Progress * Math.PI) * 0.55f : 1f;
            }
        }

        public bool TryBegin(GameAction action)
        {
            if (State != LocomotionState.Running)
            {
                return false;
            }

            if (action != GameAction.Jump && action != GameAction.Slide)
            {
                return false;
            }

            State = action == GameAction.Jump ? LocomotionState.Jumping : LocomotionState.Sliding;
            _elapsed = 0f;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            if (State == LocomotionState.Running)
            {
                return;
            }

            _elapsed += deltaTime;
            float duration = State == LocomotionState.Jumping ? _config.JumpDuration : _config.SlideDuration;
            if (_elapsed >= duration)
            {
                State = LocomotionState.Running;
                _elapsed = 0f;
            }
        }

        public void Reset()
        {
            State = LocomotionState.Running;
            _elapsed = 0f;
        }
    }
}
