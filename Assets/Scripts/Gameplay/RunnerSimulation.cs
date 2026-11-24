using System;
using IndianSubwayRunner.Core;
using UnityEngine;

namespace IndianSubwayRunner.Gameplay
{
    public sealed class RunnerSimulation : MonoBehaviour
    {
        [SerializeField] private int laneCount = 3;
        [SerializeField] private float baseSpeed = 12f;
        [SerializeField] private float maxSpeed = 30f;
        [SerializeField] private float speedRampPerSecond = 0.35f;
        [SerializeField] private float jumpDuration = 0.72f;
        [SerializeField] private float slideDuration = 0.65f;
        [SerializeField] private float laneChangeDuration = 0.16f;
        [SerializeField] private int startingLaneIndex = 1;

        private IChunkCatalog _catalog;
        private float _nextChunkDistance;
        private int _nextDialogueIndex;
        private static readonly string[] DialogueLines =
        {
            "ACP Pradyuman: Packet pakda, ab bhaag nahi bachega!",
            "Platform announcement: The next train is arriving on platform number two.",
            "ACP: Chhod na, criminal kahin nahi jayega!",
            "Announcement: Mind the gap. Mind the gap.",
            "ACP: Is baar pakad ke hi chhodunga!"
        };

        public GameConfig Config { get; private set; }
        public RunState State { get; private set; }
        public ActionArbiter Actions { get; private set; }
        public LocomotionController Locomotion { get; private set; }
        public ScoreAccumulator Score { get; private set; }
        public DifficultyDirector Difficulty { get; private set; }
        public PowerUpStateMachine PowerUps { get; private set; }
        public ChaseGap Chase { get; private set; }
        public CollisionResolver Collisions { get; private set; }
        public ChunkDirector Chunks { get; private set; }
        public ChunkDescriptor CurrentChunk { get; private set; }
        public bool IsRunning => State != null && State.Phase == RunPhase.Running;

        public event Action RunStarted;
        public event Action<int> RunEnded;
        public event Action<GameAction> ActionAccepted;
        public event Action<CollisionOutcome, CollisionContact> ContactResolved;
        public event Action<ChunkDescriptor> ChunkAdvanced;
        public event Action<string> DialogueRequested;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            Config = new GameConfig(
                laneCount,
                baseSpeed,
                maxSpeed,
                speedRampPerSecond,
                jumpDuration,
                slideDuration,
                laneChangeDuration,
                startingLaneIndex);
            State = new RunState(Config);
            Actions = new ActionArbiter(Config);
            Locomotion = new LocomotionController(Config);
            Score = new ScoreAccumulator();
            Difficulty = new DifficultyDirector();
            PowerUps = new PowerUpStateMachine();
            Chase = new ChaseGap();
            Collisions = new CollisionResolver();
            _catalog = BuildCatalog();
            Chunks = new ChunkDirector(new FixedRng(FixedSeed()), _catalog, new ChunkValidator(), Config);
            ResetRun();
        }

        public void BeginRun()
        {
            if (State.Phase != RunPhase.Ready)
            {
                return;
            }

            State.Begin();
            RunStarted?.Invoke();
        }

        public void RestartRun()
        {
            ResetRun();
            BeginRun();
        }

        public void Submit(GameAction action)
        {
            if (!IsRunning)
            {
                return;
            }

            Actions.TrySubmit(action, Time.time);
        }

        public void HandleContact(CollisionContact contact)
        {
            if (!IsRunning)
            {
                return;
            }

            CollisionOutcome outcome = Collisions.Resolve(contact, Locomotion.State, PowerUps);
            switch (outcome)
            {
                case CollisionOutcome.CollectCoin:
                    State.AddCoins(Math.Max(1, contact.Value));
                    Score.AddCoin();
                    break;
                case CollisionOutcome.CollectPowerUp:
                    PowerUps.Activate(contact.PowerUp);
                    break;
                case CollisionOutcome.Crash:
                    EndRun();
                    break;
            }

            ContactResolved?.Invoke(outcome, contact);
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            if (Actions.TryConsume(Time.time, Locomotion.State, out GameAction action))
            {
                if (action == GameAction.MoveLeft)
                {
                    State.TryChangeLane(-1);
                }
                else if (action == GameAction.MoveRight)
                {
                    State.TryChangeLane(1);
                }
                else
                {
                    Locomotion.TryBegin(action);
                }

                ActionAccepted?.Invoke(action);
            }
        }

        private void FixedUpdate()
        {
            if (!IsRunning)
            {
                return;
            }

            float deltaTime = Time.fixedDeltaTime;
            float previousDistance = State.Distance;
            Locomotion.Tick(deltaTime);
            PowerUps.Tick(deltaTime);
            Score.SetMultiplier(PowerUps.ScoreMultiplier);
            State.Tick(deltaTime);
            Score.AddDistance(State.Distance - previousDistance);

            float pressure = Mathf.InverseLerp(Config.BaseSpeed, Config.MaxSpeed, State.Speed);
            Chase.Tick(State.Distance - previousDistance, pressure);

            if (State.Distance >= _nextChunkDistance)
            {
                CurrentChunk = Chunks.Next(State.Distance);
                _nextChunkDistance += CurrentChunk.Length;
                ChunkAdvanced?.Invoke(CurrentChunk);
            }

            if (_nextDialogueIndex < DialogueLines.Length && State.Distance >= _nextDialogueIndex * 320f)
            {
                DialogueRequested?.Invoke(DialogueLines[_nextDialogueIndex]);
                _nextDialogueIndex++;
            }
        }

        private void ResetRun()
        {
            State = new RunState(Config);
            Actions.Clear();
            Locomotion.Reset();
            Score.Reset();
            PowerUps.Reset();
            Chase.Reset();
            Chunks.Reset();
            CurrentChunk = null;
            _nextChunkDistance = 0f;
            _nextDialogueIndex = 0;
        }

        private void EndRun()
        {
            if (!IsRunning)
            {
                return;
            }

            State.End();
            RunEnded?.Invoke(Score.Score);
        }

        private uint FixedSeed()
        {
            unchecked
            {
                return (uint)(laneCount * 397 + startingLaneIndex * 7919 + 20261125);
            }
        }

        private static IChunkCatalog BuildCatalog()
        {
            return new ChunkCatalog(
                CreateChunk("CHUNK_PlatformCalm_001", 12f, LaneCell.Empty, LaneCell.Coin),
                CreateChunk("CHUNK_PlatformCoins_002", 12f, LaneCell.Coin, LaneCell.Empty),
                CreateChunk("CHUNK_TicketBarrier_003", 12f, LaneCell.LowHazard, LaneCell.HighHazard),
                CreateChunk("CHUNK_TrackCross_004", 12f, LaneCell.HighHazard, LaneCell.LowHazard),
                CreateChunk("CHUNK_TrainSiding_005", 12f, LaneCell.TrainVolume, LaneCell.Empty),
                CreateChunk("CHUNK_PowerUpRun_006", 12f, LaneCell.PowerUp, LaneCell.Coin));
        }

        private static ChunkDescriptor CreateChunk(string id, float length, LaneCell primary, LaneCell secondary)
        {
            LaneCell[][] cells =
            {
                new[] { primary, secondary, LaneCell.Empty },
                new[] { LaneCell.Empty, primary, secondary },
                new[] { secondary, LaneCell.Empty, primary },
                new[] { primary, LaneCell.Empty, secondary }
            };

            bool[] safePath = { true, true, true, true };
            return new ChunkDescriptor(id, length, cells, safePath, new[] { "platform", "track" }, "calm", 100);
        }
    }
}
