using System.Collections.Generic;
using IndianSubwayRunner.Core;
using IndianSubwayRunner.Gameplay;
using UnityEngine;

namespace IndianSubwayRunner.ProceduralGeneration
{
    public sealed class ChunkView : MonoBehaviour
    {
        private const float LaneWidth = 2.6f;
        private const float SpawnZ = 32f;
        private const float CleanupZ = -60f;

        private readonly List<SpawnedChunk> _active = new List<SpawnedChunk>();
        private RunnerSimulation _simulation;
        private Material _trackMaterial;
        private Material _railMaterial;
        private Material _sleeperMaterial;
        private Material _platformMaterial;
        private Material _coinMaterial;
        private Material _lowHazardMaterial;
        private Material _highHazardMaterial;
        private Material _trainMaterial;
        private Material _powerUpMaterial;

        private void Awake()
        {
            _simulation = GetComponent<RunnerSimulation>();
            if (_simulation == null)
            {
                _simulation = FindFirstObjectByType<RunnerSimulation>();
            }

            CreateMaterials();
            _simulation.ChunkAdvanced += OnChunkAdvanced;
        }

        private void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnChunk(CreateSafeChunk(i), i * 12f);
            }
        }

        private void OnDestroy()
        {
            if (_simulation != null)
            {
                _simulation.ChunkAdvanced -= OnChunkAdvanced;
            }
        }

        private void Update()
        {
            if (_simulation == null)
            {
                return;
            }

            float travelled = _simulation.State.Distance;
            for (int chunkIndex = _active.Count - 1; chunkIndex >= 0; chunkIndex--)
            {
                SpawnedChunk chunk = _active[chunkIndex];
                float z = SpawnZ - (travelled - chunk.OriginDistance);
                chunk.Root.position = new Vector3(0f, 0f, z);

                for (int markerIndex = 0; markerIndex < chunk.Markers.Count; markerIndex++)
                {
                    ContactMarker marker = chunk.Markers[markerIndex];
                    if (marker.Triggered || travelled < marker.TriggerDistance)
                    {
                        continue;
                    }

                    marker.Triggered = true;
                    if (marker.Contact.LaneIndex == _simulation.State.LaneIndex)
                    {
                        _simulation.HandleContact(marker.Contact);
                    }
                }

                if (z < CleanupZ)
                {
                    Destroy(chunk.Root.gameObject);
                    _active.RemoveAt(chunkIndex);
                }
            }
        }

        private void OnChunkAdvanced(ChunkDescriptor descriptor)
        {
            SpawnChunk(descriptor, _simulation.State.Distance);
        }

        private void SpawnChunk(ChunkDescriptor descriptor, float originDistance)
        {
            if (descriptor == null)
            {
                return;
            }

            GameObject rootObject = new GameObject(descriptor.Id);
            Transform root = rootObject.transform;
            root.position = new Vector3(0f, 0f, SpawnZ);

            BuildTrack(root, descriptor.Length);
            List<ContactMarker> markers = new List<ContactMarker>();
            float rowLength = descriptor.Length / descriptor.LaneCells.Length;

            for (int row = 0; row < descriptor.LaneCells.Length; row++)
            {
                LaneCell[] cells = descriptor.LaneCells[row];
                float rowZ = -row * rowLength - rowLength * 0.5f;
                for (int lane = 0; lane < cells.Length; lane++)
                {
                    CollisionKind kind = MapCell(cells[lane]);
                    if (kind == CollisionKind.None)
                    {
                        continue;
                    }

                    Vector3 position = new Vector3((lane - 1) * LaneWidth, 0f, rowZ);
                    SpawnCellVisual(root, kind, position, rowLength, row);

                    PowerUpKind powerUp = row % 3 == 0
                        ? PowerUpKind.Magnet
                        : row % 3 == 1 ? PowerUpKind.ScoreMultiplier : PowerUpKind.Shield;
                    markers.Add(new ContactMarker
                    {
                        Contact = new CollisionContact(kind, lane, kind == CollisionKind.Coin ? 1 : 0, powerUp),
                        TriggerDistance = originDistance + SpawnZ + row * rowLength,
                        Triggered = false
                    });
                }
            }

            _active.Add(new SpawnedChunk
            {
                Root = root,
                OriginDistance = originDistance,
                Markers = markers
            });
        }

        private void BuildTrack(Transform root, float length)
        {
            AddPrimitive(root, "TrackBed", PrimitiveType.Cube, new Vector3(0f, -0.12f, -length * 0.5f), new Vector3(8.4f, 0.2f, length), _trackMaterial);
            AddPrimitive(root, "LeftRail", PrimitiveType.Cube, new Vector3(-1.9f, 0.04f, -length * 0.5f), new Vector3(0.08f, 0.08f, length), _railMaterial);
            AddPrimitive(root, "RightRail", PrimitiveType.Cube, new Vector3(1.9f, 0.04f, -length * 0.5f), new Vector3(0.08f, 0.08f, length), _railMaterial);
            AddPrimitive(root, "Platform", PrimitiveType.Cube, new Vector3(5.1f, 0.2f, -length * 0.5f), new Vector3(2.2f, 0.4f, length), _platformMaterial);

            int sleepers = Mathf.Max(2, Mathf.FloorToInt(length / 1.2f));
            for (int i = 0; i < sleepers; i++)
            {
                float z = -length * ((i + 0.5f) / sleepers);
                AddPrimitive(root, "Sleeper" + i, PrimitiveType.Cube, new Vector3(0f, -0.01f, z), new Vector3(7.6f, 0.06f, 0.16f), _sleeperMaterial);
            }
        }

        private void SpawnCellVisual(Transform root, CollisionKind kind, Vector3 position, float rowLength, int row)
        {
            switch (kind)
            {
                case CollisionKind.Coin:
                case CollisionKind.MagnetCoin:
                    GameObject coin = AddPrimitive(root, "Coin", PrimitiveType.Cylinder, position + new Vector3(0f, 1f, 0f), new Vector3(0.36f, 0.05f, 0.36f), _coinMaterial);
                    coin.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    break;
                case CollisionKind.LowHazard:
                    AddPrimitive(root, "LowBarrier", PrimitiveType.Cube, position + new Vector3(0f, 0.35f, 0f), new Vector3(1.5f, 0.7f, 0.7f), _lowHazardMaterial);
                    break;
                case CollisionKind.HighHazard:
                    AddPrimitive(root, "HighBarrier", PrimitiveType.Cube, position + new Vector3(0f, 0.85f, 0f), new Vector3(1.5f, 1.7f, 0.7f), _highHazardMaterial);
                    break;
                case CollisionKind.Train:
                    AddPrimitive(root, "Train", PrimitiveType.Cube, position + new Vector3(0f, 1.35f, 0f), new Vector3(2.2f, 2.7f, rowLength * 0.85f), _trainMaterial);
                    break;
                case CollisionKind.PowerUp:
                    AddPrimitive(root, "PowerUp", PrimitiveType.Sphere, position + new Vector3(0f, 1.1f, 0f), new Vector3(0.6f, 0.6f, 0.6f), _powerUpMaterial);
                    break;
            }

            if (kind == CollisionKind.Coin && row % 2 == 1)
            {
                return;
            }

            string resourcePath = MapResource(kind);
            if (string.IsNullOrEmpty(resourcePath))
            {
                return;
            }

            GameObject imported = Resources.Load<GameObject>(resourcePath);
            if (imported != null)
            {
                GameObject instance = Instantiate(imported, root);
                instance.transform.localPosition = position;
            }
        }

        private static CollisionKind MapCell(LaneCell cell)
        {
            switch (cell)
            {
                case LaneCell.Coin:
                    return CollisionKind.Coin;
                case LaneCell.LowHazard:
                    return CollisionKind.LowHazard;
                case LaneCell.HighHazard:
                    return CollisionKind.HighHazard;
                case LaneCell.TrainVolume:
                    return CollisionKind.Train;
                case LaneCell.PowerUp:
                    return CollisionKind.PowerUp;
                default:
                    return CollisionKind.None;
            }
        }

        private static string MapResource(CollisionKind kind)
        {
            switch (kind)
            {
                case CollisionKind.Coin:
                    return "Art/Models/SM_Collectibles_Coin_001";
                case CollisionKind.LowHazard:
                    return "Art/Models/SM_Obstacles_LowBarrier_001";
                case CollisionKind.HighHazard:
                    return "Art/Models/SM_Obstacles_HighBarrier_001";
                case CollisionKind.Train:
                    return "Art/Models/SM_Trains_Express_001";
                case CollisionKind.PowerUp:
                    return "Art/Models/SM_PowerUps_Shield_001";
                default:
                    return string.Empty;
            }
        }

        private static ChunkDescriptor CreateSafeChunk(int index)
        {
            LaneCell[][] cells =
            {
                new[] { LaneCell.Empty, LaneCell.Coin, LaneCell.Empty },
                new[] { LaneCell.Empty, LaneCell.Coin, LaneCell.Empty },
                new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.Empty },
                new[] { LaneCell.Empty, LaneCell.Empty, LaneCell.Empty }
            };

            return new ChunkDescriptor(
                "CHUNK_OpeningSafe_" + index,
                12f,
                cells,
                new[] { true, true, true, true },
                new[] { "platform" },
                "safe",
                0);
        }

        private void CreateMaterials()
        {
            _trackMaterial = CreateMaterial(new Color(0.24f, 0.27f, 0.31f));
            _railMaterial = CreateMaterial(new Color(0.66f, 0.68f, 0.72f));
            _sleeperMaterial = CreateMaterial(new Color(0.35f, 0.23f, 0.16f));
            _platformMaterial = CreateMaterial(new Color(0.78f, 0.76f, 0.68f));
            _coinMaterial = CreateMaterial(new Color(1f, 0.78f, 0.12f));
            _lowHazardMaterial = CreateMaterial(new Color(0.95f, 0.42f, 0.12f));
            _highHazardMaterial = CreateMaterial(new Color(0.82f, 0.12f, 0.14f));
            _trainMaterial = CreateMaterial(new Color(0.14f, 0.42f, 0.72f));
            _powerUpMaterial = CreateMaterial(new Color(0.2f, 0.86f, 0.62f));
        }

        private static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = color;
            return material;
        }

        private static GameObject AddPrimitive(
            Transform parent,
            string name,
            PrimitiveType primitive,
            Vector3 position,
            Vector3 scale,
            Material material)
        {
            GameObject instance = GameObject.CreatePrimitive(primitive);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localScale = scale;
            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            Collider collider = instance.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            return instance;
        }

        private sealed class SpawnedChunk
        {
            public Transform Root;
            public float OriginDistance;
            public List<ContactMarker> Markers;
        }

        private sealed class ContactMarker
        {
            public CollisionContact Contact;
            public float TriggerDistance;
            public bool Triggered;
        }
    }
}
