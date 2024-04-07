using System.Linq;
using Brutalsky.Base;
using Controllers;
using Core;
using UnityEngine;
using Utils;
using Utils.Constants;
using Utils.Object;
using Utils.Shape;

namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.shapePrefab;
        public override string Tag => Tags.Shape;

        public Form Form { get; set; }
        public ShapeMaterial Material { get; set; }
        public ObjectColor Color { get; set; }
        public ObjectLayer Layer { get; set; }
        public bool Simulated { get; set; }

        public BsShape(string id, ObjectTransform transform, Form form, ShapeMaterial material, ObjectColor color,
            ObjectLayer layer = ObjectLayer.Midground, bool simulated = true) : base(id, transform)
        {
            Form = form;
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
            // Link object to controller
            var controller = gameObject.GetComponent<ShapeController>();
            controller.Object = this;

            // Convert path to mesh
            var points = Form.ToPoints();
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

        protected override string[] _ToSrz()
        {
            return new[]
            {
                SrzUtils.Stringify(Transform),
                SrzUtils.Stringify(Form),
                SrzUtils.Stringify(Material),
                SrzUtils.Stringify(Color),
                SrzUtils.Stringify(Layer),
                SrzUtils.Stringify(Simulated)
            };
        }

        protected override void _FromSrz(string[] properties)
        {
            Transform = SrzUtils.ParseTransform(properties[0]);
            Form = SrzUtils.ParseForm(properties[1]);
            Material = SrzUtils.ParseMaterial(properties[2]);
            Color = SrzUtils.ParseColor(properties[3]);
            Layer = SrzUtils.ParseLayer(properties[4]);
            Simulated = SrzUtils.ParseBool(properties[5]);
        }
    }
}
