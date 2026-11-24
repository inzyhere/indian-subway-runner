using System;
using IndianSubwayRunner.Core;
using NUnit.Framework;

namespace IndianSubwayRunner.Core.Tests
{
    [TestFixture]
    public sealed class ActionArbiterTests
    {
        [Test]
        public void SubmitThenConsumeReturnsBufferedAction()
        {
            var arbiter = new ActionArbiter(new GameConfig());

            Assert.That(arbiter.TrySubmit(GameAction.Jump, 1f), Is.True);
            Assert.That(arbiter.TryConsume(1.05f, LocomotionState.Running, out GameAction action), Is.True);
            Assert.That(action, Is.EqualTo(GameAction.Jump));
        }

        [Test]
        public void SecondSubmitBeforeConsumeIsRejected()
        {
            var arbiter = new ActionArbiter(new GameConfig());

            Assert.That(arbiter.TrySubmit(GameAction.MoveLeft, 0f), Is.True);
            Assert.That(arbiter.TrySubmit(GameAction.Jump, 0.01f), Is.False);
        }

        [Test]
        public void ExpiredBufferIsCleared()
        {
            var arbiter = new ActionArbiter(new GameConfig());

            Assert.That(arbiter.TrySubmit(GameAction.Slide, 0f), Is.True);
            Assert.That(arbiter.TryConsume(0.2f, LocomotionState.Running, out _), Is.False);
            Assert.That(arbiter.TryConsume(0.21f, LocomotionState.Running, out _), Is.False);
        }

        [Test]
        public void BufferIsNotConsumedDuringAirOrSlide()
        {
            var arbiter = new ActionArbiter(new GameConfig());

            Assert.That(arbiter.TrySubmit(GameAction.MoveRight, 0f), Is.True);
            Assert.That(arbiter.TryConsume(0.01f, LocomotionState.Jumping, out _), Is.False);
            Assert.That(arbiter.TryConsume(0.02f, LocomotionState.Sliding, out _), Is.False);
            Assert.That(arbiter.TryConsume(0.03f, LocomotionState.Running, out GameAction action), Is.True);
            Assert.That(action, Is.EqualTo(GameAction.MoveRight));
        }
    }

    [TestFixture]
    public sealed class LocomotionControllerTests
    {
        [Test]
        public void JumpBlocksSecondActionAndReturnsToRunning()
        {
            var controller = new LocomotionController(new GameConfig());

            Assert.That(controller.TryBegin(GameAction.Jump), Is.True);
            Assert.That(controller.State, Is.EqualTo(LocomotionState.Jumping));
            Assert.That(controller.TryBegin(GameAction.Slide), Is.False);
            controller.Tick(0.36f);
            Assert.That(controller.VerticalOffset, Is.GreaterThan(2f));
            controller.Tick(0.36f);
            Assert.That(controller.State, Is.EqualTo(LocomotionState.Running));
        }

        [Test]
        public void SlideLowersHeightAndReturnsToRunning()
        {
            var controller = new LocomotionController(new GameConfig());

            Assert.That(controller.TryBegin(GameAction.Slide), Is.True);
            controller.Tick(0.325f);
            Assert.That(controller.HeightScale, Is.LessThan(0.5f));
            controller.Tick(0.325f);
            Assert.That(controller.State, Is.EqualTo(LocomotionState.Running));
        }

        [Test]
        public void LaneMovesDoNotChangeLocomotionState()
        {
            var controller = new LocomotionController(new GameConfig());

            Assert.That(controller.TryBegin(GameAction.MoveLeft), Is.False);
            Assert.That(controller.State, Is.EqualTo(LocomotionState.Running));
        }
    }

    [TestFixture]
    public sealed class ScoreAccumulatorTests
    {
        [Test]
        public void DistanceAndCoinsUseMultiplier()
        {
            var score = new ScoreAccumulator();
            score.SetMultiplier(2);

            score.AddDistance(12.5f);
            score.AddCoin();

            Assert.That(score.Score, Is.EqualTo(450));
        }

        [Test]
        public void SubPointDistanceDoesNotAwardScore()
        {
            var score = new ScoreAccumulator();

            score.AddDistance(0.09f);

            Assert.That(score.Score, Is.EqualTo(0));
        }

        [Test]
        public void InvalidMultiplierThrows()
        {
            var score = new ScoreAccumulator();

            Assert.Throws<ArgumentOutOfRangeException>(() => score.SetMultiplier(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => score.SetMultiplier(5));
        }
    }

    [TestFixture]
    public sealed class DifficultyDirectorTests
    {
        [Test]
        public void ProfileScalesWithDistance()
        {
            var director = new DifficultyDirector();
            SpawnProfile calm = director.GetProfile(0f);
            SpawnProfile peak = director.GetProfile(3000f);

            Assert.That(peak.ObstacleChance, Is.GreaterThan(calm.ObstacleChance));
            Assert.That(peak.TrainChance, Is.GreaterThan(calm.TrainChance));
            Assert.That(peak.CoinChance, Is.LessThan(calm.CoinChance));
            Assert.That(peak.PowerUpChance, Is.GreaterThan(calm.PowerUpChance));
        }

        [Test]
        public void BandsAdvanceMonotonically()
        {
            var director = new DifficultyDirector();

            Assert.That(director.GetBand(100f), Is.EqualTo(DifficultyBand.Calm));
            Assert.That(director.GetBand(500f), Is.EqualTo(DifficultyBand.Building));
            Assert.That(director.GetBand(1500f), Is.EqualTo(DifficultyBand.Busy));
            Assert.That(director.GetBand(2800f), Is.EqualTo(DifficultyBand.Peak));
        }
    }

    [TestFixture]
    public sealed class PowerUpStateMachineTests
    {
        [Test]
        public void TimedEffectsExpire()
        {
            var powerUps = new PowerUpStateMachine();
            powerUps.Activate(PowerUpKind.Magnet);
            powerUps.Activate(PowerUpKind.ScoreMultiplier);

            Assert.That(powerUps.IsMagnetActive, Is.True);
            Assert.That(powerUps.ScoreMultiplier, Is.EqualTo(2));

            powerUps.Tick(9f);

            Assert.That(powerUps.IsMagnetActive, Is.False);
            Assert.That(powerUps.IsMultiplierActive, Is.False);
            Assert.That(powerUps.ScoreMultiplier, Is.EqualTo(1));
        }

        [Test]
        public void ShieldIsConsumedOnce()
        {
            var powerUps = new PowerUpStateMachine();
            powerUps.Activate(PowerUpKind.Shield);

            Assert.That(powerUps.HasShield, Is.True);
            Assert.That(powerUps.TryConsumeShield(), Is.True);
            Assert.That(powerUps.TryConsumeShield(), Is.False);
        }
    }

    [TestFixture]
    public sealed class ChaseGapTests
    {
        [Test]
        public void GapNeverClosesBelowMinimum()
        {
            var chase = new ChaseGap(initial: 9f, minimum: 8f, maximum: 32f);

            chase.Tick(1000f, 1f);

            Assert.That(chase.Current, Is.EqualTo(8f));
        }

        [Test]
        public void GapNeverExceedsMaximum()
        {
            var chase = new ChaseGap(initial: 30f, minimum: 8f, maximum: 32f);

            chase.Tick(1000f, 0f);

            Assert.That(chase.Current, Is.EqualTo(32f));
        }
    }

    [TestFixture]
    public sealed class CollisionResolverTests
    {
        [Test]
        public void CorrectActionIgnoresMatchingHazard()
        {
            var resolver = new CollisionResolver();
            var powerUps = new PowerUpStateMachine();

            Assert.That(
                resolver.Resolve(new CollisionContact(CollisionKind.LowHazard, 1), LocomotionState.Jumping, powerUps),
                Is.EqualTo(CollisionOutcome.Ignore));
            Assert.That(
                resolver.Resolve(new CollisionContact(CollisionKind.HighHazard, 1), LocomotionState.Sliding, powerUps),
                Is.EqualTo(CollisionOutcome.Ignore));
        }

        [Test]
        public void WrongActionCrashesWithoutShield()
        {
            var resolver = new CollisionResolver();
            var powerUps = new PowerUpStateMachine();

            Assert.That(
                resolver.Resolve(new CollisionContact(CollisionKind.HighHazard, 1), LocomotionState.Running, powerUps),
                Is.EqualTo(CollisionOutcome.Crash));
        }

        [Test]
        public void ShieldAbsorbsFatalImpact()
        {
            var resolver = new CollisionResolver();
            var powerUps = new PowerUpStateMachine();
            powerUps.Activate(PowerUpKind.Shield);

            Assert.That(
                resolver.Resolve(new CollisionContact(CollisionKind.Train, 1), LocomotionState.Running, powerUps),
                Is.EqualTo(CollisionOutcome.ShieldAbsorbed));
            Assert.That(powerUps.HasShield, Is.False);
        }

        [Test]
        public void CoinsAndPowerUpsAreCollected()
        {
            var resolver = new CollisionResolver();
            var powerUps = new PowerUpStateMachine();

            Assert.That(
                resolver.Resolve(new CollisionContact(CollisionKind.Coin, 1), LocomotionState.Running, powerUps),
                Is.EqualTo(CollisionOutcome.CollectCoin));
            Assert.That(
                resolver.Resolve(new CollisionContact(CollisionKind.PowerUp, 1), LocomotionState.Running, powerUps),
                Is.EqualTo(CollisionOutcome.CollectPowerUp));
        }
    }

    [TestFixture]
    public sealed class FixedRngTests
    {
        [Test]
        public void SameSeedProducesSameSequence()
        {
            var first = new FixedRng(42u);
            var second = new FixedRng(42u);

            for (int i = 0; i < 16; i++)
            {
                Assert.That(first.NextUInt32(), Is.EqualTo(second.NextUInt32()));
            }
        }

        [Test]
        public void NextIntStaysInRange()
        {
            var rng = new FixedRng(7u);

            for (int i = 0; i < 128; i++)
            {
                int value = rng.NextInt(2, 5);
                Assert.That(value, Is.InRange(2, 4));
            }
        }

        [Test]
        public void ZeroSeedIsNormalized()
        {
            var rng = new FixedRng(0u);

            Assert.That(rng.NextUInt32(), Is.Not.EqualTo(0u));
        }
    }

    [TestFixture]
    public sealed class ChunkValidatorTests
    {
        private static readonly GameConfig Config = new GameConfig();

        [Test]
        public void SafeThreeLaneChunkIsValid()
        {
            LaneCell[][] cells =
            {
                new[] { LaneCell.Empty, LaneCell.Coin, LaneCell.LowHazard },
                new[] { LaneCell.Empty, LaneCell.Coin, LaneCell.Empty },
                new[] { LaneCell.HighHazard, LaneCell.Empty, LaneCell.Empty },
                new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.PowerUp }
            };

            Assert.That(ChunkValidatorHelpers.MakeChunk("VALID", cells, 12f).Id, Is.EqualTo("VALID"));
            Assert.That(new ChunkValidator().IsValid(ChunkValidatorHelpers.MakeChunk("VALID", cells, 12f), Config), Is.True);
        }

        [Test]
        public void FullyBlockedRowIsInvalid()
        {
            LaneCell[][] cells =
            {
                new[] { LaneCell.LowHazard, LaneCell.HighHazard, LaneCell.TrainVolume },
                new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.Empty }
            };

            ChunkDescriptor chunk = ChunkValidatorHelpers.MakeChunk("BLOCKED", cells, 12f);

            Assert.That(new ChunkValidator().IsValid(chunk, Config), Is.False);
        }

        [Test]
        public void ImpossibleLaneJumpIsInvalid()
        {
            LaneCell[][] cells =
            {
                new[] { LaneCell.Empty, LaneCell.HighHazard, LaneCell.HighHazard },
                new[] { LaneCell.HighHazard, LaneCell.HighHazard, LaneCell.Empty },
                new[] { LaneCell.HighHazard, LaneCell.HighHazard, LaneCell.Empty },
                new[] { LaneCell.HighHazard, LaneCell.HighHazard, LaneCell.Empty }
            };

            ChunkDescriptor chunk = ChunkValidatorHelpers.MakeChunk("IMPOSSIBLE", cells, 3f);

            Assert.That(new ChunkValidator().IsValid(chunk, Config), Is.False);
        }

        [Test]
        public void ReactionTimeLongerThanRowTravelTimeIsInvalid()
        {
            LaneCell[][] cells =
            {
                new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.Empty },
                new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.Empty }
            };

            ChunkDescriptor chunk = ChunkValidatorHelpers.MakeChunk("SLOW", cells, 12f, minReactionMs: 1000);

            Assert.That(new ChunkValidator().IsValid(chunk, Config), Is.False);
        }
    }

    [TestFixture]
    public sealed class ChunkDirectorTests
    {
        [Test]
        public void SameSeedProducesSameChunkOrder()
        {
            ChunkCatalog catalog = ChunkValidatorHelpers.MakeCatalog(3);
            var first = new ChunkDirector(new FixedRng(11u), catalog, new ChunkValidator(), new GameConfig());
            var second = new ChunkDirector(new FixedRng(11u), catalog, new ChunkValidator(), new GameConfig());

            for (int i = 0; i < 12; i++)
            {
                Assert.That(first.Next(i * 10f).Id, Is.EqualTo(second.Next(i * 10f).Id));
            }
        }

        [Test]
        public void RecentChunkIsNotRepeated()
        {
            ChunkCatalog catalog = ChunkValidatorHelpers.MakeCatalog(3);
            var director = new ChunkDirector(new FixedRng(5u), catalog, new ChunkValidator(), new GameConfig());
            string previous = string.Empty;

            for (int i = 0; i < 12; i++)
            {
                string current = director.Next(i * 10f).Id;
                Assert.That(current, Is.Not.EqualTo(previous));
                previous = current;
            }
        }

        [Test]
        public void EmptyCatalogReturnsFallback()
        {
            var director = new ChunkDirector(new FixedRng(1u), new ChunkCatalog(), new ChunkValidator(), new GameConfig());

            Assert.That(director.Next(0f).Id, Is.EqualTo("CHUNK_FallbackSafe_001"));
        }
    }

    internal static class ChunkValidatorHelpers
    {
        public static ChunkDescriptor MakeChunk(
            string id,
            LaneCell[][] cells,
            float length,
            int minReactionMs = 0)
        {
            bool[] safePath = new bool[cells.Length];
            for (int row = 0; row < cells.Length; row++)
            {
                safePath[row] = true;
            }

            return new ChunkDescriptor(
                id,
                length,
                cells,
                safePath,
                new[] { "any" },
                "calm",
                minReactionMs);
        }

        public static ChunkCatalog MakeCatalog(int count)
        {
            var chunks = new ChunkDescriptor[count];
            for (int i = 0; i < count; i++)
            {
                LaneCell[][] cells =
                {
                    new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.Empty },
                    new[] { LaneCell.Coin, LaneCell.Empty, LaneCell.Empty },
                    new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.PowerUp },
                    new[] { LaneCell.Empty, LaneCell.Coin, LaneCell.Empty }
                };

                chunks[i] = MakeChunk("CHUNK_Valid_" + i, cells, 12f);
            }

            return new ChunkCatalog(chunks);
        }
    }
}
