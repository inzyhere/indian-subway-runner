namespace IndianSubwayRunner.Core
{
    public interface IChunkCatalog
    {
        int Count { get; }

        ChunkDescriptor Get(int index);
    }
}
