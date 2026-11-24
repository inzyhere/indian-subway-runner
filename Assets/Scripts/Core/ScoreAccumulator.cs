using System;

namespace IndianSubwayRunner.Core
{
    public sealed class ScoreAccumulator
    {
        private int _score;
        private int _multiplier = 1;

        public int Score => _score;

        public int Multiplier => _multiplier;

        public void AddDistance(float deltaDistance)
        {
            if (deltaDistance < 0f || float.IsNaN(deltaDistance) || float.IsInfinity(deltaDistance))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaDistance));
            }

            int points = (int)Math.Floor(deltaDistance * 10f);
            if (points <= 0)
            {
                return;
            }

            _score = checked(_score + points * _multiplier);
        }

        public void AddCoin()
        {
            _score = checked(_score + 100 * _multiplier);
        }

        public void SetMultiplier(int multiplier)
        {
            if (multiplier < 1 || multiplier > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier));
            }

            _multiplier = multiplier;
        }

        public void Reset()
        {
            _score = 0;
            _multiplier = 1;
        }
    }
}
