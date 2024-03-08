using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Joint;
using Brutalsky.Object;
using Controllers;
using Controllers.Shape;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Core
{
    public class MapSystem : MonoBehaviour
    {
        public static MapSystem current;
        private void Awake() => current = this;

        // Constants
        public const string SaveFormat = "yaml";

        // Variables
        public Dictionary<uint, string> rawMapList { get; private set; } = new();
        [CanBeNull] public BsMap activeMap { get; private set; }
        public bool mapLoaded { get; private set; }

        // References
        public GameObject mapMargins;
        public Light2D cMapLight2D;
        public GameObject shapePrefab;
        public GameObject poolPrefab;
        public Material litMaterial;
        public Material unlitMaterial;
        private GameObject mapParent;

        // Functions
        public void GenerateMapList(bool local = false)
        {
            var pathBase = local ? EventSystem.ContentPath : EventSystem.dataPath;
            var path = $"{pathBase}/{EventSystem.MapsFolder}";
            foreach (var mapPath in Directory.GetFiles(path, $"*.{SaveFormat}"))
            {
                var mapFilename = Regex.Match(mapPath, $@"\w+(?=\.{SaveFormat})").Value;
                var rawMap = Load(mapFilename, local);
                var map = BsMap.Parse(rawMap);
                rawMapList[map.id] = rawMap;
            }
        }

        public static string Load(string filename, bool local = false)
        {
            var pathBase = local ? EventSystem.ContentPath : EventSystem.dataPath;
            var path = $"{pathBase}/{EventSystem.MapsFolder}/{filename}.{SaveFormat}";
            using var reader = new StreamReader(path);
            return reader.ReadToEnd();
        }

        public static void Save(string raw, string filename, bool local = false)
        {
            var pathBase = local ? EventSystem.ContentPath : EventSystem.dataPath;
            var path = $"{pathBase}/{EventSystem.MapsFolder}/{filename}.{SaveFormat}";
            new FileInfo(path).Directory?.Create();
            using var writer = new StreamWriter(path);
            writer.Write(raw);
        }

        public void Rebuild()
        {
            if (!mapLoaded) return;
            var map = activeMap;
            Unbuild();
            Build(map);
        }

        public void Build(BsMap map)
        {
            // Make sure there is not already an active map
            if (mapLoaded) return;

            // Note map as active
            activeMap = map;
            mapLoaded = true;

            // Set camera and lighting config
            CameraSystem.current.ResizeView(map.size);
            mapMargins.transform.localScale = map.size;
            cMapLight2D.color = map.lighting.tint;
            cMapLight2D.intensity = map.lighting.alpha;

            // Instantiate the map container and create all objects
            mapParent = new GameObject();
            foreach (var shape in map.shapes.Values)
            {
                Create(shape);
            }
            foreach (var pool in map.pools.Values)
            {
                Create(pool);
            }
            foreach (var joint in map.joints.Values)
            {
                Create(joint);
            }
        }

        public void Unbuild()
        {
            // Make sure there is an active map to unbuild
            if (!mapLoaded) return;

            // Delete all objects and destroy the map container
            foreach (var joint in activeMap.joints.Values)
            {
                Delete(joint);
            }
            foreach (var pool in activeMap.pools.Values)
            {
                Delete(pool);
            }
            foreach (var shape in activeMap.shapes.Values)
            {
                Delete(shape);
            }
            Destroy(mapParent);
            mapParent = null;

            // Note map as inactive
            activeMap = null;
            mapLoaded = false;
        }

        public bool Create(BsShape shape)
        {
            // Make sure the shape is not already created
            if (shape.active) return false;

            // Create new object
            var shapeObject = Instantiate(shapePrefab, mapParent.transform);
            var shapeController = shapeObject.GetComponent<ShapeController>();
            shapeController.bsObject = shape;

            // Convert path to mesh
            var points = shape.path.ToPoints();
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
            meshRenderer.material = shape.color.glow ? unlitMaterial : litMaterial;
            meshRenderer.material.color = shape.color.tint;
            meshRenderer.sortingOrder = Layer2Order(shape.layer);

            // Apply material
            var rigidbody = shapeObject.GetComponent<Rigidbody2D>();
            if (shape.simulated)
            {
                var physicsMaterial = new PhysicsMaterial2D
                {
                    friction = shape.material.friction,
                    bounciness = shape.material.restitution
                };
                polygonCollider.sharedMaterial = physicsMaterial;
                rigidbody.sharedMaterial = physicsMaterial;
                if (shape.material.dynamic)
                {
                    rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    polygonCollider.density = shape.material.density;
                }
            }
            else
            {
                rigidbody.simulated = false;
            }

            // Apply position and rotation
            shapeObject.transform.position = shape.transform.position;
            shapeObject.transform.rotation = Quaternion.Euler(0f, 0f, shape.transform.rotation);

            // Note shape as active
            shape.instanceObject = shapeObject;
            shape.instanceComponent = shapeController;
            shape.active = true;
            return true;
        }

        public bool Create(BsPool pool)
        {
            // Make sure the pool is not already created
            if (pool.active) return false;

            // Create new object
            var poolObject = Instantiate(poolPrefab, mapParent.transform);
            var poolController = poolObject.GetComponent<PoolController>();
            poolController.bsObject = pool;

            // Apply size
            poolObject.transform.localScale = pool.size;

            // Apply color and layer
            var spriteRenderer = poolObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = pool.color.glow ? unlitMaterial : litMaterial;
            spriteRenderer.material.color = pool.color.tint;
            spriteRenderer.sortingOrder = Layer2Order(pool.layer);

            // Apply chemical
            if (!pool.simulated)
            {
                poolObject.GetComponent<BoxCollider2D>().enabled = false;
                poolController.enabled = false;
            }

            // Apply position and rotation
            poolObject.transform.position = pool.transform.position;
            poolObject.transform.rotation = Quaternion.Euler(0f, 0f, pool.transform.rotation);

            // Note pool as active
            pool.instanceObject = poolObject;
            pool.instanceComponent = poolController;
            pool.active = true;
            return true;
        }

        public bool Create(BsJoint joint)
        {
            // Make sure the joint is not already created
            if (joint.active) return false;

            // Create new component
            AnchoredJoint2D jointComponent;
            var targetShape = activeMap.GetShape(joint.targetShapeId);
            var mountShape = joint.mountShapeId.Length > 0 ? activeMap.GetShape(joint.mountShapeId) : null;
            if (targetShape == null)
            {
                throw Errors.NoTargetShape(joint);
            }
            var targetGameObject = targetShape.instanceObject;
            if (targetGameObject == null)
            {
                throw Errors.TargetShapeUnbuilt(joint);
            }

            // Apply joint config
            switch (joint.jointType)
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
                    throw Errors.InvalidJointType(joint.jointType);
            }
            joint.ApplyConfigToInstance(jointComponent);

            // Note joint as active
            joint.instanceComponent = jointComponent;
            joint.active = true;
            return true;
        }

        public bool Delete(BsShape shape)
        {
            // Make sure the shape is not already deleted
            if (!shape.active) return false;

            // Destroy the shape object
            Destroy(shape.instanceObject);
            
            // Note shape as inactive
            shape.instanceObject = null;
            shape.active = false;
            return true;
        }

        public bool Delete(BsPool pool)
        {
            // Make sure the pool is not already deleted
            if (!pool.active) return false;

            // Destroy the pool object
            Destroy(pool.instanceObject);
            
            // Note pool as inactive
            pool.instanceObject = null;
            pool.active = false;
            return true;
        }

        public bool Delete(BsJoint joint)
        {
            // Make sure the joint is not already deleted
            if (!joint.active) return false;

            // Destroy the joint component
            Destroy(joint.instanceComponent);
            
            // Note joint as inactive
            joint.instanceComponent = null;
            joint.active = false;
            return true;
        }

        // Events
        private void Start()
        {
            GenerateMapList(true);
            GenerateMapList();
        }

        // Utilities
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
