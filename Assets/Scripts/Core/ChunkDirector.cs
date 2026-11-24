using System;

namespace IndianSubwayRunner.Core
{
    public sealed class ChunkDirector
    {
        private readonly IRng _rng;
        private readonly IChunkCatalog _catalog;
        private readonly ChunkValidator _validator;
        private readonly GameConfig _config;
        private readonly int[] _recent = new int[2];
        private int _recentCount;

        public ChunkDirector(IRng rng, IChunkCatalog catalog, ChunkValidator validator, GameConfig config)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public ChunkDescriptor Next(float distance)
        {
            if (distance < 0f || float.IsNaN(distance) || float.IsInfinity(distance))
            {
                throw new ArgumentOutOfRangeException(nameof(distance));
            }

            int count = _catalog.Count;
            if (count == 0)
            {
                return CreateFallback();
            }

            int attempts = count * 4;
            for (int attempt = 0; attempt < attempts; attempt++)
            {
                int index = _rng.NextInt(0, count);
                if (WasRecent(index) || !_validator.IsValid(_catalog.Get(index), _config))
                {
                    continue;
                }

                Remember(index);
                return _catalog.Get(index);
            }

            return CreateFallback();
        }

        public void Reset()
        {
            _recentCount = 0;
            _recent[0] = -1;
            _recent[1] = -1;
        }

        private bool WasRecent(int index)
        {
            for (int i = 0; i < _recentCount; i++)
            {
                if (_recent[i] == index)
                {
                    return true;
                }
            }

            return false;
        }

        private void Remember(int index)
        {
            if (_recentCount == _recent.Length)
            {
                _recent[1] = _recent[0];
                _recent[0] = index;
                return;
            }

            _recent[_recentCount++] = index;
        }

        private static ChunkDescriptor CreateFallback()
        {
            LaneCell[][] cells = new LaneCell[4][];
            for (int row = 0; row < cells.Length; row++)
            {
                cells[row] = new LaneCell[3];
            }

            return new ChunkDescriptor(
                id: "CHUNK_FallbackSafe_001",
                length: 12f,
                laneCells: cells,
                safePath: new[] { true, true, true, true },
                themeTags: new[] { "any" },
                difficultyTag: "safe",
                minReactionMs: 0);
        }
    }
}
