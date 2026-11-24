using IndianSubwayRunner.Gameplay;
using UnityEngine;

namespace IndianSubwayRunner.Player
{
    public sealed class RunnerView : MonoBehaviour
    {
        private RunnerSimulation _simulation;
        private Transform _root;
        private Transform _model;
        private float _visualLane;

        private void Awake()
        {
            _simulation = GetComponent<RunnerSimulation>();
            if (_simulation == null)
            {
                _simulation = FindFirstObjectByType<RunnerSimulation>();
            }

            Build();
        }

        private void LateUpdate()
        {
            if (_simulation == null || _root == null || _model == null)
            {
                return;
            }

            float targetLane = (float)_simulation.State.LaneIndex - 1f;
            float laneSpeed = 1f / Mathf.Max(0.01f, _simulation.Config.LaneChangeDuration);
            _visualLane = Mathf.MoveTowards(_visualLane, targetLane, Time.deltaTime * laneSpeed);

            Vector3 position = _root.position;
            position.x = _visualLane * 2.6f;
            position.y = _simulation.Locomotion.VerticalOffset;
            _root.position = position;

            Vector3 scale = _model.localScale;
            scale.y = Mathf.Max(0.2f, _simulation.Locomotion.HeightScale);
            _model.localScale = scale;
        }

        private void Build()
        {
            GameObject rootObject = new GameObject("RunnerAvatar");
            _root = rootObject.transform;
            _visualLane = _simulation == null ? 0f : (float)_simulation.State.LaneIndex - 1f;

            GameObject imported = Resources.Load<GameObject>("Art/Models/SM_Characters_Runner_001");
            if (imported != null)
            {
                GameObject instance = Instantiate(imported, _root);
                _model = instance.transform;
                _model.localPosition = new Vector3(0f, 0.9f, 0f);
                return;
            }

            _model = BuildPrimitiveRunner(_root);
        }

        private static Transform BuildPrimitiveRunner(Transform parent)
        {
            GameObject modelObject = new GameObject("RunnerPrimitive");
            Transform model = modelObject.transform;
            model.SetParent(parent, false);

            AddPart(model, "Torso", PrimitiveType.Capsule, new Vector3(0f, 0.95f, 0f), new Vector3(0.55f, 0.8f, 0.35f), new Color(0.15f, 0.45f, 0.85f));
            AddPart(model, "Head", PrimitiveType.Sphere, new Vector3(0f, 1.75f, 0f), new Vector3(0.42f, 0.42f, 0.42f), new Color(0.86f, 0.65f, 0.46f));
            AddPart(model, "Cap", PrimitiveType.Cylinder, new Vector3(0f, 2f, 0f), new Vector3(0.46f, 0.08f, 0.46f), new Color(0.95f, 0.25f, 0.18f));
            AddPart(model, "LeftArm", PrimitiveType.Capsule, new Vector3(-0.42f, 1f, 0f), new Vector3(0.18f, 0.55f, 0.18f), new Color(0.9f, 0.85f, 0.72f));
            AddPart(model, "RightArm", PrimitiveType.Capsule, new Vector3(0.42f, 1f, 0f), new Vector3(0.18f, 0.55f, 0.18f), new Color(0.9f, 0.85f, 0.72f));
            AddPart(model, "LeftLeg", PrimitiveType.Capsule, new Vector3(-0.22f, 0.32f, 0f), new Vector3(0.2f, 0.55f, 0.2f), new Color(0.16f, 0.2f, 0.32f));
            AddPart(model, "RightLeg", PrimitiveType.Capsule, new Vector3(0.22f, 0.32f, 0f), new Vector3(0.2f, 0.55f, 0.2f), new Color(0.16f, 0.2f, 0.32f));
            return model;
        }

        private static void AddPart(
            Transform parent,
            string name,
            PrimitiveType primitive,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject part = GameObject.CreatePrimitive(primitive);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;

            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = CreateMaterial(color);
            }

            Collider collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
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
    }
}
