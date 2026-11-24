using NUnit.Framework;

namespace IndianSubwayRunner.Core.Tests
{
    public sealed class RunStateTests
    {
        [Test]
        public void RunStartsInCenterLane()
        {
            var state = new RunState(new GameConfig());

            Assert.AreEqual(RunPhase.Ready, state.Phase);
            Assert.AreEqual(1, state.LaneIndex);
        }

        [Test]
        public void LaneChangeRejectsOutOfRangeDirection()
        {
            var state = new RunState(new GameConfig());
            state.Begin();

            Assert.IsFalse(state.TryChangeLane(0));
            Assert.IsFalse(state.TryChangeLane(-2));
            Assert.AreEqual(1, state.LaneIndex);
        }

        [Test]
        public void LaneChangeMovesWithinBounds()
        {
            var state = new RunState(new GameConfig());
            state.Begin();

            Assert.IsTrue(state.TryChangeLane(-1));
            Assert.AreEqual(0, state.LaneIndex);
            Assert.IsFalse(state.TryChangeLane(-1));
        }

        [Test]
        public void TickAdvancesDistanceAndSpeed()
        {
            var state = new RunState(new GameConfig(speedRampPerSecond: 1f));
            state.Begin();

            state.Tick(1f);

            Assert.AreEqual(13f, state.Speed, 0.0001f);
            Assert.AreEqual(13f, state.Distance, 0.0001f);
        }

        [Test]
        public void SpeedDoesNotExceedMaximum()
        {
            var state = new RunState(new GameConfig(baseSpeed: 10f, maxSpeed: 11f, speedRampPerSecond: 5f));
            state.Begin();

            state.Tick(10f);

            Assert.AreEqual(11f, state.Speed, 0.0001f);
        }

        [Test]
        public void ScoreIncludesDistanceAndCoins()
        {
            var state = new RunState(new GameConfig(baseSpeed: 10f, speedRampPerSecond: 0f));
            state.Begin();

            state.Tick(1f);
            state.AddCoins(2);

            Assert.AreEqual(210, state.CalculateScore());
        }

        [Test]
        public void EndedRunDoesNotAdvance()
        {
            var state = new RunState(new GameConfig());
            state.Begin();
            state.Tick(1f);
            state.End();

            float distance = state.Distance;
            state.Tick(1f);

            Assert.AreEqual(distance, state.Distance, 0.0001f);
        }
    }
}
