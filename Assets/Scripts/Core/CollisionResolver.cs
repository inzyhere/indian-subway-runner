using System;

namespace IndianSubwayRunner.Core
{
    public enum CollisionKind
    {
        None,
        Coin,
        MagnetCoin,
        LowHazard,
        HighHazard,
        Train,
        PowerUp
    }

    public enum CollisionOutcome
    {
        Ignore,
        CollectCoin,
        CollectPowerUp,
        Stumble,
        Crash,
        ShieldAbsorbed
    }

    public readonly struct CollisionContact
    {
        public CollisionContact(CollisionKind kind, int laneIndex, int value = 0, PowerUpKind powerUp = PowerUpKind.Magnet)
        {
            Kind = kind;
            LaneIndex = laneIndex;
            Value = value;
            PowerUp = powerUp;
        }

        public CollisionKind Kind { get; }

        public int LaneIndex { get; }

        public int Value { get; }

        public PowerUpKind PowerUp { get; }
    }

    public sealed class CollisionResolver
    {
        public CollisionOutcome Resolve(
            CollisionContact contact,
            LocomotionState state,
            PowerUpStateMachine powerUps)
        {
            if (powerUps == null)
            {
                throw new ArgumentNullException(nameof(powerUps));
            }

            switch (contact.Kind)
            {
                case CollisionKind.None:
                    return CollisionOutcome.Ignore;
                case CollisionKind.Coin:
                case CollisionKind.MagnetCoin:
                    return CollisionOutcome.CollectCoin;
                case CollisionKind.PowerUp:
                    return CollisionOutcome.CollectPowerUp;
                case CollisionKind.LowHazard:
                    if (state == LocomotionState.Jumping)
                    {
                        return CollisionOutcome.Ignore;
                    }

                    return powerUps.TryConsumeShield() ? CollisionOutcome.ShieldAbsorbed : CollisionOutcome.Stumble;
                case CollisionKind.HighHazard:
                    if (state == LocomotionState.Sliding)
                    {
                        return CollisionOutcome.Ignore;
                    }

                    return powerUps.TryConsumeShield() ? CollisionOutcome.ShieldAbsorbed : CollisionOutcome.Crash;
                case CollisionKind.Train:
                    return powerUps.TryConsumeShield() ? CollisionOutcome.ShieldAbsorbed : CollisionOutcome.Crash;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contact));
            }
        }
    }
}
