using System;

namespace IndianSubwayRunner.Core
{
    public enum LaneCell
    {
        Empty,
        Coin,
        LowHazard,
        HighHazard,
        TrainVolume,
        PowerUp
    }

    public sealed class ChunkDescriptor
    {
        public ChunkDescriptor(
            string id,
            float length,
            LaneCell[][] laneCells,
            bool[] safePath,
            string[] themeTags,
            string difficultyTag,
            int minReactionMs)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("A chunk id is required.", nameof(id));
            }

            if (length <= 0f || float.IsNaN(length) || float.IsInfinity(length))
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (laneCells == null)
            {
                throw new ArgumentNullException(nameof(laneCells));
            }

            if (laneCells.Length == 0)
            {
                throw new ArgumentException("At least one row is required.", nameof(laneCells));
            }

            if (safePath == null || safePath.Length != laneCells.Length)
            {
                throw new ArgumentException("SafePath must match the row count.", nameof(safePath));
            }

            if (themeTags == null || themeTags.Length == 0)
            {
                throw new ArgumentException("At least one theme tag is required.", nameof(themeTags));
            }

            if (string.IsNullOrWhiteSpace(difficultyTag))
            {
                throw new ArgumentException("A difficulty tag is required.", nameof(difficultyTag));
            }

            if (minReactionMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minReactionMs));
            }

            Id = id;
            Length = length;
            LaneCells = new LaneCell[laneCells.Length][];
            for (int row = 0; row < laneCells.Length; row++)
            {
                LaneCell[] source = laneCells[row];
                if (source == null || source.Length == 0)
                {
                    throw new ArgumentException("Every row must contain at least one lane.", nameof(laneCells));
                }

                LaneCells[row] = new LaneCell[source.Length];
                Array.Copy(source, LaneCells[row], source.Length);
            }

            SafePath = new bool[safePath.Length];
            Array.Copy(safePath, SafePath, safePath.Length);
            ThemeTags = new string[themeTags.Length];
            Array.Copy(themeTags, ThemeTags, themeTags.Length);
            DifficultyTag = difficultyTag;
            MinReactionMs = minReactionMs;
        }

        public string Id { get; }

        public float Length { get; }

        public LaneCell[][] LaneCells { get; }

        public bool[] SafePath { get; }

        public string[] ThemeTags { get; }

        public string DifficultyTag { get; }

        public int MinReactionMs { get; }
    }
}
