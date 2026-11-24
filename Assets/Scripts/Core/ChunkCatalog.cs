using System;

namespace IndianSubwayRunner.Core
{
    public sealed class ChunkCatalog : IChunkCatalog
    {
        private readonly ChunkDescriptor[] _chunks;

        public ChunkCatalog(params ChunkDescriptor[] chunks)
        {
            _chunks = chunks ?? throw new ArgumentNullException(nameof(chunks));
            for (int i = 0; i < _chunks.Length; i++)
            {
                if (_chunks[i] == null)
                {
                    throw new ArgumentException("A chunk catalog cannot contain null entries.", nameof(chunks));
                }
            }
        }

        public int Count => _chunks.Length;

        public ChunkDescriptor Get(int index)
        {
            if (index < 0 || index >= _chunks.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _chunks[index];
        }
    }
}
