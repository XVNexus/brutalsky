using System;
using System.Linq;
using Brutalsky;
using Controllers;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

namespace Core
{
    public class MapSystem : MonoBehaviour
    {
        public static MapSystem current;
        private void Awake() => current = this;

        // Variables
        [CanBeNull] public BsMap activeMap { get; private set; }
        public bool mapLoaded { get; private set; }

        // Functions
        public void Load(BsMap map)
        {
            if (mapLoaded) return;
            activeMap = map;
            mapLoaded = true;

            foreach (var shape in activeMap.shapes)
            {
                Create(shape);
            }
            foreach (var pool in activeMap.pools)
            {
                Create(pool);
            }
        }

        public void Unload()
        {
            if (!mapLoaded) return;
            activeMap = null;
            mapLoaded = false;
        }

        public bool Spawn(BsPlayer player)
        {
            if (!mapLoaded || player.active) return false;

            // Create new object and apply color and health
            var playerObj = Instantiate(PrefabSystem.current.player);
            playerObj.GetComponent<PlayerColorController>().playerColor = player.color;
            playerObj.GetComponent<PlayerHealthController>().maxHealth = player.health;

            // Select a spawnpoint
            var spawnPos = activeMap.SelectSpawn();
            playerObj.transform.position = spawnPos;

            player.instance = playerObj;
            player.active = true;
            return true;
        }

        public bool Despawn(BsPlayer player)
        {
            if (!player.active) return false;

            Destroy(player.instance);
            player.instance = null;
            player.active = false;
            return true;
        }

        public bool Create(BsShape shape)
        {
            if (shape.active) return false;

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

            // Create new object and apply mesh
            var shapeObj = Instantiate(PrefabSystem.current.shape);
            shapeObj.GetComponent<MeshFilter>().mesh = mesh;
            var polygonCollider = shapeObj.GetComponent<PolygonCollider2D>();
            polygonCollider.SetPath(0, points);

            // Apply color
            var meshRenderer = shapeObj.GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = shape.color.sortingOrder;
            meshRenderer.material.color = shape.color.tint;

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

            shape.instance = shapeObj;
            shape.active = true;
            return true;
        }

        public bool Create(BsPool pool)
        {
            if (pool.active) return false;

            // Create new object and apply size
            var poolObj = Instantiate(PrefabSystem.current.pool);
            poolObj.transform.localScale = pool.size;

            // Apply color
            var spriteRenderer = poolObj.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = pool.color.sortingOrder;
            spriteRenderer.material.color = pool.color.tint;

            // Apply chemical
            var poolController = poolObj.GetComponent<PoolController>();
            if (pool.simulated)
            {
                poolController.buoyancy = pool.chemical.buoyancy;
                poolController.viscosity = pool.chemical.viscosity;
            }
            else
            {
                poolObj.GetComponent<BoxCollider2D>().enabled = false;
                poolController.enabled = false;
            }

            // Apply position and rotation
            poolObj.transform.position = pool.transform.position;
            poolObj.transform.rotation = Quaternion.Euler(0f, 0f, pool.transform.rotation);

            pool.instance = poolObj;
            pool.active = true;
            return true;
        }

        public bool Create(BsJoint joint)
        {
            throw new NotImplementedException();
        }

        public bool Destroy(BsShape shape)
        {
            if (!shape.active) return false;

            Destroy(shape.instance);
            shape.instance = null;
            shape.active = false;
            return true;
        }

        public bool Destroy(BsPool pool)
        {
            if (!pool.active) return false;

            Destroy(pool.instance);
            pool.instance = null;
            pool.active = false;
            return true;
        }

        public bool Destroy(BsJoint joint)
        {
            throw new NotImplementedException();
        }
    }
}
