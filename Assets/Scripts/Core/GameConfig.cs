namespace IndianSubwayRunner.Core
{
    public sealed class GameConfig
    {
        public int LaneCount { get; }
        public float BaseSpeed { get; }
        public float MaxSpeed { get; }
        public float SpeedRampPerSecond { get; }
        public float JumpDuration { get; }
        public float SlideDuration { get; }
        public float LaneChangeDuration { get; }
        public int StartingLaneIndex { get; }

        public GameConfig(
            int laneCount = 3,
            float baseSpeed = 12f,
            float maxSpeed = 30f,
            float speedRampPerSecond = 0.35f,
            float jumpDuration = 0.72f,
            float slideDuration = 0.65f,
            float laneChangeDuration = 0.16f,
            int startingLaneIndex = 1)
        {
            if (laneCount < 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(laneCount));
            }

            if (baseSpeed <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(nameof(baseSpeed));
            }

            if (maxSpeed < baseSpeed)
            {
                throw new System.ArgumentOutOfRangeException(nameof(maxSpeed));
            }

            if (speedRampPerSecond < 0f)
            {
                throw new System.ArgumentOutOfRangeException(nameof(speedRampPerSecond));
            }

            if (jumpDuration <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(nameof(jumpDuration));
            }

            if (slideDuration <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(nameof(slideDuration));
            }

            if (laneChangeDuration <= 0f)
            {
                throw new System.ArgumentOutOfRangeException(nameof(laneChangeDuration));
            }

            if (startingLaneIndex < 0 || startingLaneIndex >= laneCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(startingLaneIndex));
            }

            LaneCount = laneCount;
            BaseSpeed = baseSpeed;
            MaxSpeed = maxSpeed;
            SpeedRampPerSecond = speedRampPerSecond;
            JumpDuration = jumpDuration;
            SlideDuration = slideDuration;
            LaneChangeDuration = laneChangeDuration;
            StartingLaneIndex = startingLaneIndex;
        }
    }
}
