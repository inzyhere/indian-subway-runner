using System;

namespace IndianSubwayRunner.Core
{
    public enum DifficultyBand
    {
        Calm,
        Building,
        Busy,
        Peak
    }

    public readonly struct SpawnProfile
    {
        public SpawnProfile(float obstacleChance, float trainChance, float coinChance, float powerUpChance)
        {
            ObstacleChance = obstacleChance;
            TrainChance = trainChance;
            CoinChance = coinChance;
            PowerUpChance = powerUpChance;
        }

        public float ObstacleChance { get; }

        public float TrainChance { get; }

        public float CoinChance { get; }

        public float PowerUpChance { get; }
    }

    public sealed class DifficultyDirector
    {
        private const float PeakDistance = 3000f;

        public SpawnProfile GetProfile(float distance)
        {
            ValidateDistance(distance);

            float t = Math.Min(1f, distance / PeakDistance);
            return new SpawnProfile(
                obstacleChance: 0.12f + 0.32f * t,
                trainChance: 0.03f + 0.10f * t,
                coinChance: 0.45f - 0.12f * t,
                powerUpChance: 0.03f + 0.02f * t);
        }

        public DifficultyBand GetBand(float distance)
        {
            ValidateDistance(distance);

            if (distance < 350f)
            {
                return DifficultyBand.Calm;
            }

            if (distance < 1000f)
            {
                return DifficultyBand.Building;
            }

            if (distance < 2200f)
            {
                return DifficultyBand.Busy;
            }

            return DifficultyBand.Peak;
        }

        private static void ValidateDistance(float distance)
        {
            if (distance < 0f || float.IsNaN(distance) || float.IsInfinity(distance))
            {
                throw new ArgumentOutOfRangeException(nameof(distance));
            }
        }
    }
}
