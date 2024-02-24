using System;
using System.Collections.Generic;
using System.Linq;
using Brutalsky;
using Brutalsky.Joint;
using Brutalsky.Object;
using Controllers;
using Controllers.Player;
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
        public const float AnimationTime = 1f;

        // Variables
        [CanBeNull] public BsMap activeMap { get; private set; }
        public bool mapLoaded { get; private set; }
        public Dictionary<string, BsPlayer> activePlayers { get; private set; } = new();

        // References
        public Light2D cMapLight2D;
        public GameObject playerPrefab;
        public GameObject shapePrefab;
        public GameObject poolPrefab;
        public Material litMaterial;
        public Material unlitMaterial;
        private GameObject mapParent;

        // Functions
        public void LoadLevel(BsMap map, IEnumerable<BsPlayer> players)
        {
            BuildMap(map);
            SpawnPlayers(players);
        }

        public void ChangeLevel(BsMap newMap)
        {
            throw new NotImplementedException();
        }

        public void UnloadLevel()
        {
            DespawnPlayers();
            UnbuildMap();
        }

        public void FreezePlayers()
        {
            foreach (var player in activePlayers.Values)
            {
                player.instanceObject.GetComponent<PlayerController>().Freeze();
            }
        }

        public void UnfreezePlayers()
        {
            foreach (var player in activePlayers.Values)
            {
                player.instanceObject.GetComponent<PlayerController>().Unfreeze();
            }
        }

        public void BuildMap(BsMap map)
        {
            if (mapLoaded) return;
            activeMap = map;
            mapLoaded = true;

            CameraSystem.current.viewSize = map.size;
            mapParent = new GameObject();
            cMapLight2D.color = map.lighting.tint;
            cMapLight2D.intensity = map.lighting.alpha;
            foreach (var shape in map.shapes.Values)
            {
                CreateObject(shape);
            }
            foreach (var pool in map.pools.Values)
            {
                CreateObject(pool);
            }
            foreach (var joint in map.joints.Values)
            {
                CreateObject(joint);
            }

            EventSystem.current.TriggerMapLoad(map);
        }

        public void UnbuildMap()
        {
            if (!mapLoaded) return;
            
            EventSystem.current.TriggerMapUnload(activeMap);

            foreach (var joint in activeMap.joints.Values)
            {
                DeleteObject(joint);
            }
            foreach (var pool in activeMap.pools.Values)
            {
                DeleteObject(pool);
            }
            foreach (var shape in activeMap.shapes.Values)
            {
                DeleteObject(shape);
            }
            Destroy(mapParent);
            mapParent = null;

            activeMap = null;
            mapLoaded = false;
        }

        public void SpawnPlayers(IEnumerable<BsPlayer> players)
        {
            var playerList = players.ToList();
            while (playerList.Count > 0)
            {
                var index = EventSystem.random.NextInt(playerList.Count);
                SpawnPlayer(playerList[index]);
                playerList.RemoveAt(index);
            }
        }

        public bool SpawnPlayer(BsPlayer player)
        {
            if (!mapLoaded || player.active) return false;

            // Create new object and apply color and health
            var playerObject = Instantiate(playerPrefab);
            var playerController = playerObject.GetComponent<PlayerController>();
            playerController.bsObject = player;
            playerController.maxHealth = player.health;
            playerController.color = player.color.tint;
            if (player.dummy)
            {
                playerObject.GetComponent<PlayerMovementController>().movementForce = 0f;
            }

            // Select a spawnpoint
            var spawnPos = activeMap.SelectSpawn();
            playerObject.transform.position = spawnPos;

            player.instanceObject = playerObject;
            player.active = true;
            activePlayers[player.id] = player;

            EventSystem.current.TriggerPlayerSpawn(activeMap, player);

            return true;
        }

        public void DespawnPlayers()
        {
            DespawnPlayers(activePlayers.Values);
        }

        public void DespawnPlayers(IEnumerable<BsPlayer> players)
        {
            var playerList = players.ToList();
            foreach (var player in playerList)
            {
                DespawnPlayer(player);
            }
        }

        public bool DespawnPlayer(BsPlayer player)
        {
            if (!player.active) return false;

            EventSystem.current.TriggerPlayerDespawn(player);

            Destroy(player.instanceObject);
            player.instanceObject = null;
            player.active = false;
            activePlayers.Remove(player.id);
            return true;
        }

        public bool CreateObject(BsShape shape)
        {
            if (!mapLoaded || shape.active) return false;

            // Create new object
            var shapeObj = Instantiate(shapePrefab, mapParent.transform);
            var shapeController = shapeObj.GetComponent<ShapeController>();
            shapeController.bsObject = shape;

            // Convert path to mesh
            var points = shape.path.ToPoints();
            var vertices = points.Select(point => (Vector3)point).ToArray();
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new Triangulator(points).Triangulate()
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Apply mesh
            shapeObj.GetComponent<MeshFilter>().mesh = mesh;
            var polygonCollider = shapeObj.GetComponent<PolygonCollider2D>();
            polygonCollider.SetPath(0, points);

            // Apply color and layer
            var meshRenderer = shapeObj.GetComponent<MeshRenderer>();
            meshRenderer.material = shape.color.glow ? unlitMaterial : litMaterial;
            meshRenderer.material.color = shape.color.tint;
            meshRenderer.sortingOrder = Layer2Order(shape.layer);

            // Apply material
            var rigidbody = shapeObj.GetComponent<Rigidbody2D>();
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
            shapeObj.transform.position = shape.transform.position;
            shapeObj.transform.rotation = Quaternion.Euler(0f, 0f, shape.transform.rotation);

            shape.instanceObject = shapeObj;
            shape.active = true;
            return true;
        }

        public bool CreateObject(BsPool pool)
        {
            if (!mapLoaded || pool.active) return false;

            // Create new object
            var poolObj = Instantiate(poolPrefab, mapParent.transform);
            var poolController = poolObj.GetComponent<PoolController>();
            poolController.bsObject = pool;

            // Apply size
            poolObj.transform.localScale = pool.size;

            // Apply color and layer
            var spriteRenderer = poolObj.GetComponent<SpriteRenderer>();
            spriteRenderer.material = pool.color.glow ? unlitMaterial : litMaterial;
            spriteRenderer.material.color = pool.color.tint;
            spriteRenderer.sortingOrder = Layer2Order(pool.layer);

            // Apply chemical
            if (!pool.simulated)
            {
                poolObj.GetComponent<BoxCollider2D>().enabled = false;
                poolController.enabled = false;
            }

            // Apply position and rotation
            poolObj.transform.position = pool.transform.position;
            poolObj.transform.rotation = Quaternion.Euler(0f, 0f, pool.transform.rotation);

            pool.instanceObject = poolObj;
            pool.active = true;
            return true;
        }

        public bool CreateObject(BsJoint joint)
        {
            if (!mapLoaded || joint.active) return false;

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

            joint.instanceComponent = jointComponent;
            joint.active = true;
            return true;
        }

        public bool DeleteObject(BsShape shape)
        {
            if (!shape.active) return false;

            Destroy(shape.instanceObject);
            shape.instanceObject = null;
            shape.active = false;
            return true;
        }

        public bool DeleteObject(BsPool pool)
        {
            if (!pool.active) return false;

            Destroy(pool.instanceObject);
            pool.instanceObject = null;
            pool.active = false;
            return true;
        }

        public bool DeleteObject(BsJoint joint)
        {
            if (!joint.active) return false;

            Destroy(joint.instanceComponent);
            joint.instanceComponent = null;
            joint.active = false;
            return true;
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
