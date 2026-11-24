using System;

namespace IndianSubwayRunner.Core
{
    public enum RunPhase
    {
        Ready,
        Running,
        Ended
    }

    public sealed class RunState
    {
        private readonly GameConfig _config;

        public RunPhase Phase { get; private set; }
        public int LaneIndex { get; private set; }
        public float Distance { get; private set; }
        public int Coins { get; private set; }
        public float Speed { get; private set; }

        public RunState(GameConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            Phase = RunPhase.Ready;
            LaneIndex = config.StartingLaneIndex;
            Speed = config.BaseSpeed;
        }

        public void Begin()
        {
            if (Phase == RunPhase.Ready)
            {
                Phase = RunPhase.Running;
            }
        }

        public bool TryChangeLane(int direction)
        {
            if (Phase != RunPhase.Running || direction < -1 || direction > 1 || direction == 0)
            {
                return false;
            }

            int next = LaneIndex + direction;
            if (next < 0 || next >= _config.LaneCount)
            {
                return false;
            }

            LaneIndex = next;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (Phase != RunPhase.Running)
            {
                return;
            }

            if (deltaTime <= 0f || float.IsNaN(deltaTime) || float.IsInfinity(deltaTime))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            Speed = Math.Min(_config.MaxSpeed, Speed + _config.SpeedRampPerSecond * deltaTime);
            Distance += Speed * deltaTime;
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Coins = checked(Coins + amount);
        }

        public int CalculateScore()
        {
            int distanceScore = checked((int)(Distance * 10f));
            return checked(distanceScore + Coins * 100);
        }

        public void End()
        {
            if (Phase == RunPhase.Running)
            {
                Phase = RunPhase.Ended;
            }
        }
    }
}
