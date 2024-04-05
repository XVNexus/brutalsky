using System.Collections.Generic;
using System.Linq;
using Brutalsky.Base;
using Controllers;
using Core;
using UnityEngine;
using Utils;
using Utils.Constants;
using Utils.Object;
using Utils.Path;
using Utils.Shape;

namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.shapePrefab;
        public override string Tag => Tags.Shape;
        public Path Path { get; set; }
        public ShapeMaterial Material { get; set; }
        public ObjectColor Color { get; set; }
        public ObjectLayer Layer { get; set; }
        public bool Simulated { get; set; }

        public BsShape(string id, ObjectTransform transform, Path path, ShapeMaterial material, ObjectColor color,
            ObjectLayer layer = ObjectLayer.Midground, bool simulated = true) : base(id, transform)
        {
            Path = path;
            Material = material;
            Color = color;
            Layer = layer;
            Simulated = simulated;
        }

        public BsShape()
        {
        }

        protected override Component _Init(GameObject gameObject, BsMap map)
        {
            // Create new object
            var controller = gameObject.GetComponent<ShapeController>();
            controller.Object = this;

            // Convert path to mesh
            var points = Path.ToPoints();
            var vertices = points.Select(point => (Vector3)point).ToArray();
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new ShapeTriangulator(points).Triangulate()
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Apply mesh
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            var polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
            polygonCollider.SetPath(0, points);

            // Apply color and layer
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = Color.Glow ? ResourceSystem._.unlitMaterial : ResourceSystem._.litMaterial;
            meshRenderer.material.color = Color.Tint;
            meshRenderer.sortingOrder = BsUtils.Layer2Order(Layer);

            // Apply material
            var rigidbody = gameObject.GetComponent<Rigidbody2D>();
            if (Simulated)
            {
                var physicsMaterial = new PhysicsMaterial2D
                {
                    friction = Material.Friction,
                    bounciness = Material.Restitution
                };
                polygonCollider.sharedMaterial = physicsMaterial;
                rigidbody.sharedMaterial = physicsMaterial;
                if (Material.Dynamic)
                {
                    rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    polygonCollider.density = Material.Density;
                }
            }
            else
            {
                rigidbody.simulated = false;
            }

            // Apply position and rotation
            gameObject.transform.position = Transform.Position;
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Transform.Rotation);

            return controller;
        }

        protected override Dictionary<string, string> _ToSrz()
        {
            throw new System.NotImplementedException();
        }

        protected override void _FromSrz(Dictionary<string, string> properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
