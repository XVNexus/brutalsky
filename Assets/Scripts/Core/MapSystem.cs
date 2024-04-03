using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Joint;
using Brutalsky.Object;
using Controllers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;
using Utils.Shape;

namespace Core
{
    public class MapSystem : BsBehavior
    {
        // Static instance
        public static MapSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Local constants
        public const string SaveFormat = "yaml";

        // Local variables
        public Dictionary<uint, string> RawMapList { get; private set; } = new();
        [CanBeNull] public BsMap ActiveMap { get; private set; }
        public bool IsMapLoaded { get; private set; }

        // External references
        public GameObject mapMargins;
        public Light2D cMapLight2D;
        public GameObject shapePrefab;
        public GameObject poolPrefab;
        public Material litMaterial;
        public Material unlitMaterial;
        private GameObject _mapParent;

        // Init functions
        protected override void OnStart()
        {
            GenerateMapList();
        }

        public void GenerateMapList()
        {
            RawMapList.Clear();
            var defaultRawMap = LoadInternal("Brutalsky");
            var defaultMap = BsMap.Parse(defaultRawMap);
            RawMapList[defaultMap.Id] = defaultRawMap;
            var path = $"{EventSystem.DataPath}/Maps";
            if (!Directory.Exists(path)) return;
            foreach (var mapPath in Directory.GetFiles(path, $"*.{SaveFormat}"))
            {
                var mapFilename = Regex.Match(mapPath, $@"\w+(?=\.{SaveFormat})").Value;
                var rawMap = Load(mapFilename);
                var map = BsMap.Parse(rawMap);
                RawMapList[map.Id] = rawMap;
            }
        }

        public static string LoadInternal(string filename)
        {
            return Resources.Load<TextAsset>($"Content/Maps/{filename}").text;
        }

        public static string Load(string filename)
        {
            var path = $"{EventSystem.DataPath}/Maps/{filename}.{SaveFormat}";
            using var reader = new StreamReader(path);
            return reader.ReadToEnd();
        }

        public static void Save(string raw, string filename)
        {
            var path = $"{EventSystem.DataPath}/Maps/{filename}.{SaveFormat}";
            new FileInfo(path).Directory?.Create();
            using var writer = new StreamWriter(path);
            writer.Write(raw);
        }

        public void Rebuild()
        {
            if (!IsMapLoaded) return;
            var map = ActiveMap;
            Unbuild();
            Build(map);
        }

        public void Build(BsMap map)
        {
            // Make sure there is not already an active map
            if (IsMapLoaded) return;

            // Note map as active
            ActiveMap = map;
            IsMapLoaded = true;

            // Set camera and lighting config
            CameraSystem._.ResizeView(map.Size);
            mapMargins.transform.localScale = map.Size;
            cMapLight2D.color = map.Lighting.Tint;
            cMapLight2D.intensity = map.Lighting.Alpha;

            // Instantiate the map container and create all objects
            _mapParent = new GameObject();
            foreach (var shape in map.Shapes.Values)
            {
                Create(shape);
            }
            foreach (var pool in map.Pools.Values)
            {
                Create(pool);
            }
            foreach (var joint in map.Joints.Values)
            {
                Create(joint);
            }
        }

        public void Unbuild()
        {
            // Make sure there is an active map to unbuild
            if (!IsMapLoaded) return;

            // Delete all objects and destroy the map container
            foreach (var joint in ActiveMap.Joints.Values)
            {
                Delete(joint);
            }
            foreach (var pool in ActiveMap.Pools.Values)
            {
                Delete(pool);
            }
            foreach (var shape in ActiveMap.Shapes.Values)
            {
                Delete(shape);
            }
            Destroy(_mapParent);
            _mapParent = null;

            // Note map as inactive
            ActiveMap = null;
            IsMapLoaded = false;
        }

        public bool Create(BsShape shape)
        {
            // Make sure the shape is not already created
            if (shape.Active) return false;

            // Create new object
            var shapeObject = Instantiate(shapePrefab, _mapParent.transform);
            var shapeController = shapeObject.GetComponent<ShapeController>();
            shapeController.Object = shape;

            // Convert path to mesh
            var points = shape.Path.ToPoints();
            var vertices = points.Select(point => (Vector3)point).ToArray();
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new ShapeTriangulator(points).Triangulate()
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Apply mesh
            shapeObject.GetComponent<MeshFilter>().mesh = mesh;
            var polygonCollider = shapeObject.GetComponent<PolygonCollider2D>();
            polygonCollider.SetPath(0, points);

            // Apply color and layer
            var meshRenderer = shapeObject.GetComponent<MeshRenderer>();
            meshRenderer.material = shape.Color.Glow ? unlitMaterial : litMaterial;
            meshRenderer.material.color = shape.Color.Tint;
            meshRenderer.sortingOrder = Layer2Order(shape.Layer);

            // Apply material
            var rigidbody = shapeObject.GetComponent<Rigidbody2D>();
            if (shape.Simulated)
            {
                var physicsMaterial = new PhysicsMaterial2D
                {
                    friction = shape.Material.Friction,
                    bounciness = shape.Material.Restitution
                };
                polygonCollider.sharedMaterial = physicsMaterial;
                rigidbody.sharedMaterial = physicsMaterial;
                if (shape.Material.Dynamic)
                {
                    rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    polygonCollider.density = shape.Material.Density;
                }
            }
            else
            {
                rigidbody.simulated = false;
            }

            // Apply position and rotation
            shapeObject.transform.position = shape.Transform.Position;
            shapeObject.transform.rotation = Quaternion.Euler(0f, 0f, shape.Transform.Rotation);

            // Note shape as active
            shape.InstanceObject = shapeObject;
            shape.InstanceComponent = shapeController;
            shape.Active = true;
            return true;
        }

        public bool Create(BsPool pool)
        {
            // Make sure the pool is not already created
            if (pool.Active) return false;

            // Create new object
            var poolObject = Instantiate(poolPrefab, _mapParent.transform);
            var poolController = poolObject.GetComponent<PoolController>();
            poolController.Object = pool;

            // Apply size
            poolObject.transform.localScale = pool.Size;

            // Apply color and layer
            var spriteRenderer = poolObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = pool.Color.Glow ? unlitMaterial : litMaterial;
            spriteRenderer.material.color = pool.Color.Tint;
            spriteRenderer.sortingOrder = Layer2Order(pool.Layer);

            // Apply chemical
            if (!pool.Simulated)
            {
                poolObject.GetComponent<BoxCollider2D>().enabled = false;
                poolController.enabled = false;
            }

            // Apply position and rotation
            poolObject.transform.position = pool.Transform.Position;
            poolObject.transform.rotation = Quaternion.Euler(0f, 0f, pool.Transform.Rotation);

            // Note pool as active
            pool.InstanceObject = poolObject;
            pool.InstanceComponent = poolController;
            pool.Active = true;
            return true;
        }

        public bool Create(BsJoint joint)
        {
            // Make sure the joint is not already created
            if (joint.Active) return false;

            // Create new component
            AnchoredJoint2D jointComponent;
            var targetShape = ActiveMap.GetShape(joint.TargetShapeId);
            var mountShape = joint.MountShapeId.Length > 0 ? ActiveMap.GetShape(joint.MountShapeId) : null;
            if (targetShape == null)
            {
                throw Errors.NoTargetShape(joint);
            }
            var targetGameObject = targetShape.InstanceObject;
            if (targetGameObject == null)
            {
                throw Errors.TargetShapeUnbuilt(joint);
            }

            // Apply joint config
            switch (joint.JointType)
            {
                case BsJointType.Fixed:
                    jointComponent = targetGameObject.AddComponent<FixedJoint2D>();
                    ((BsJointFixed)joint).ApplyBaseConfigToInstance(jointComponent, mountShape);
                    break;
                case BsJointType.Distance:
                    jointComponent = targetGameObject.AddComponent<DistanceJoint2D>();
                    ((BsJointDistance)joint).ApplyBaseConfigToInstance(jointComponent, mountShape);
                    break;
                case BsJointType.Spring:
                    jointComponent = targetGameObject.AddComponent<SpringJoint2D>();
                    ((BsJointSpring)joint).ApplyBaseConfigToInstance(jointComponent, mountShape);
                    break;
                case BsJointType.Hinge:
                    jointComponent = targetGameObject.AddComponent<HingeJoint2D>();
                    ((BsJointHinge)joint).ApplyBaseConfigToInstance(jointComponent, mountShape);
                    break;
                case BsJointType.Slider:
                    jointComponent = targetGameObject.AddComponent<SliderJoint2D>();
                    ((BsJointSlider)joint).ApplyBaseConfigToInstance(jointComponent, mountShape);
                    break;
                case BsJointType.Wheel:
                    jointComponent = targetGameObject.AddComponent<WheelJoint2D>();
                    ((BsJointWheel)joint).ApplyBaseConfigToInstance(jointComponent, mountShape);
                    break;
                case BsJointType.None:
                default:
                    throw Errors.InvalidJointType(joint.JointType);
            }
            joint.ApplyConfigToInstance(jointComponent);

            // Note joint as active
            joint.InstanceComponent = jointComponent;
            joint.Active = true;
            return true;
        }

        public bool Delete(BsShape shape)
        {
            // Make sure the shape is not already deleted
            if (!shape.Active) return false;

            // Destroy the shape object
            Destroy(shape.InstanceObject);

            // Note shape as inactive
            shape.InstanceObject = null;
            shape.Active = false;
            return true;
        }

        public bool Delete(BsPool pool)
        {
            // Make sure the pool is not already deleted
            if (!pool.Active) return false;

            // Destroy the pool object
            Destroy(pool.InstanceObject);

            // Note pool as inactive
            pool.InstanceObject = null;
            pool.Active = false;
            return true;
        }

        public bool Delete(BsJoint joint)
        {
            // Make sure the joint is not already deleted
            if (!joint.Active) return false;

            // Destroy the joint component
            Destroy(joint.InstanceComponent);

            // Note joint as inactive
            joint.InstanceComponent = null;
            joint.Active = false;
            return true;
        }

        public static int Layer2Order(BsLayer layer)
        {
            return layer switch
            {
                BsLayer.Background => -2,
                BsLayer.Foreground => 2,
                _ => 0
            };
        }
    }
}
