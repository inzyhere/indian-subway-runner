using System;

namespace IndianSubwayRunner.Core
{
    public sealed class ChaseGap
    {
        private readonly float _minimum;
        private readonly float _maximum;
        private float _current;

        public ChaseGap(float initial = 18f, float minimum = 8f, float maximum = 32f)
        {
            if (minimum < 0f || maximum <= minimum || float.IsNaN(minimum) || float.IsInfinity(minimum))
            {
                throw new ArgumentOutOfRangeException(nameof(minimum));
            }

            if (float.IsNaN(maximum) || float.IsInfinity(maximum))
            {
                throw new ArgumentOutOfRangeException(nameof(maximum));
            }

            if (float.IsNaN(initial) || float.IsInfinity(initial))
            {
                throw new ArgumentOutOfRangeException(nameof(initial));
            }

            _minimum = minimum;
            _maximum = maximum;
            _current = Math.Min(maximum, Math.Max(minimum, initial));
        }

        public float Current => _current;

        public void Tick(float deltaDistance, float pressure)
        {
            if (deltaDistance < 0f || float.IsNaN(deltaDistance) || float.IsInfinity(deltaDistance))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaDistance));
            }

            if (pressure < 0f || pressure > 1f || float.IsNaN(pressure) || float.IsInfinity(pressure))
            {
                throw new ArgumentOutOfRangeException(nameof(pressure));
            }

            float change = (0.5f - pressure) * deltaDistance * 0.25f;
            _current = Math.Min(_maximum, Math.Max(_minimum, _current + change));
        }

        public void Reset(float initial = 18f)
        {
            if (float.IsNaN(initial) || float.IsInfinity(initial))
            {
                throw new ArgumentOutOfRangeException(nameof(initial));
            }

            _current = Math.Min(_maximum, Math.Max(_minimum, initial));
        }
    }
}
