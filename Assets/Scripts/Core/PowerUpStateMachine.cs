using System;

namespace IndianSubwayRunner.Core
{
    public enum PowerUpKind
    {
        Magnet,
        ScoreMultiplier,
        Shield
    }

    public sealed class PowerUpStateMachine
    {
        private const float EffectDuration = 8f;
        private float _magnetRemaining;
        private float _multiplierRemaining;
        private bool _hasShield;

        public bool IsMagnetActive => _magnetRemaining > 0f;

        public bool IsMultiplierActive => _multiplierRemaining > 0f;

        public bool HasShield => _hasShield;

        public int ScoreMultiplier => IsMultiplierActive ? 2 : 1;

        public void Activate(PowerUpKind kind)
        {
            switch (kind)
            {
                case PowerUpKind.Magnet:
                    _magnetRemaining = Math.Max(_magnetRemaining, EffectDuration);
                    break;
                case PowerUpKind.ScoreMultiplier:
                    _multiplierRemaining = Math.Max(_multiplierRemaining, EffectDuration);
                    break;
                case PowerUpKind.Shield:
                    _hasShield = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f || float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            _magnetRemaining = Math.Max(0f, _magnetRemaining - deltaTime);
            _multiplierRemaining = Math.Max(0f, _multiplierRemaining - deltaTime);
        }

        public bool TryConsumeShield()
        {
            if (!_hasShield)
            {
                return false;
            }

            _hasShield = false;
            return true;
        }

        public void Reset()
        {
            _magnetRemaining = 0f;
            _multiplierRemaining = 0f;
            _hasShield = false;
        }
    }
}
