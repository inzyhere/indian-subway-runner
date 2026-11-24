using System;

namespace IndianSubwayRunner.Core
{
    public sealed class FixedRng : IRng
    {
        private uint _state;

        public FixedRng(uint seed)
        {
            _state = seed == 0u ? 0x9E3779B9u : seed;
        }

        public uint NextUInt32()
        {
            uint value = _state;
            value ^= value << 13;
            value ^= value >> 17;
            value ^= value << 5;
            _state = value;
            return value;
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));
            }

            uint span = (uint)(maxExclusive - minInclusive);
            return minInclusive + (int)(NextUInt32() % span);
        }

        public float NextFloat()
        {
            return NextUInt32() / 4294967296f;
        }
    }
}
