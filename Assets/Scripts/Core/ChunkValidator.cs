using System;

namespace IndianSubwayRunner.Core
{
    public sealed class ChunkValidator
    {
        public bool IsValid(ChunkDescriptor chunk, GameConfig config)
        {
            if (chunk == null || config == null)
            {
                return false;
            }

            int rows = chunk.LaneCells.Length;
            float rowTimeMs = (chunk.Length / rows) / config.BaseSpeed * 1000f;
            if (rowTimeMs <= 0f)
            {
                return false;
            }

            int previousSafeMask = 0;
            for (int row = 0; row < rows; row++)
            {
                LaneCell[] cells = chunk.LaneCells[row];
                if (cells.Length != config.LaneCount)
                {
                    return false;
                }

                int safeMask = 0;
                int hazards = 0;
                for (int lane = 0; lane < cells.Length; lane++)
                {
                    switch (cells[lane])
                    {
                        case LaneCell.Empty:
                        case LaneCell.Coin:
                        case LaneCell.PowerUp:
                            safeMask |= 1 << lane;
                            break;
                        case LaneCell.LowHazard:
                        case LaneCell.HighHazard:
                        case LaneCell.TrainVolume:
                            hazards++;
                            break;
                        default:
                            return false;
                    }
                }

                if (safeMask == 0 || !chunk.SafePath[row])
                {
                    return false;
                }

                if (hazards >= cells.Length)
                {
                    return false;
                }

                if (chunk.MinReactionMs > rowTimeMs)
                {
                    return false;
                }

                if (row > 0 && !HasReachableLane(previousSafeMask, safeMask, rowTimeMs, config.LaneChangeDuration))
                {
                    return false;
                }

                previousSafeMask = safeMask;
            }

            return true;
        }

        private static bool HasReachableLane(int previousMask, int currentMask, float rowTimeMs, float laneChangeDuration)
        {
            int minimumChanges = 3;
            for (int from = 0; from < 8; from++)
            {
                if ((previousMask & (1 << from)) == 0)
                {
                    continue;
                }

                for (int to = 0; to < 8; to++)
                {
                    if ((currentMask & (1 << to)) == 0)
                    {
                        continue;
                    }

                    int changes = Math.Abs(from - to);
                    if (changes < minimumChanges)
                    {
                        minimumChanges = changes;
                    }
                }
            }

            return minimumChanges * laneChangeDuration * 1000f <= rowTimeMs;
        }
    }
}
