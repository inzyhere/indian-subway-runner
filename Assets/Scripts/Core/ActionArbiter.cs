using System;

namespace IndianSubwayRunner.Core
{
    public sealed class ActionArbiter
    {
        private readonly float _bufferWindow;
        private bool _hasBuffered;
        private GameAction _bufferedAction;
        private float _bufferedAt;

        public ActionArbiter(GameConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _bufferWindow = Math.Min(0.12f, Math.Max(0.08f, config.LaneChangeDuration));
        }

        public bool TrySubmit(GameAction action, float timeSeconds)
        {
            ValidateTime(timeSeconds);

            if (_hasBuffered)
            {
                return false;
            }

            _bufferedAction = action;
            _bufferedAt = timeSeconds;
            _hasBuffered = true;
            return true;
        }

        public bool TryConsume(float timeSeconds, LocomotionState state, out GameAction action)
        {
            ValidateTime(timeSeconds);
            action = default;

            if (!_hasBuffered || state != LocomotionState.Running || timeSeconds < _bufferedAt)
            {
                return false;
            }

            if (timeSeconds - _bufferedAt > _bufferWindow)
            {
                Clear();
                return false;
            }

            action = _bufferedAction;
            Clear();
            return true;
        }

        public void Clear()
        {
            _hasBuffered = false;
            _bufferedAction = default;
            _bufferedAt = 0f;
        }

        private static void ValidateTime(float timeSeconds)
        {
            if (timeSeconds < 0f || float.IsNaN(timeSeconds) || float.IsInfinity(timeSeconds))
            {
                throw new ArgumentOutOfRangeException(nameof(timeSeconds));
            }
        }
    }
}
