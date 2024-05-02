using System.Linq;
using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;
using Utils.Shape;

namespace Brutalsky.Object
{
    public class BsShape : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pShape;
        public override string Tag => Tags.Shape;

        public Form Form { get; set; }
        public ShapeMaterial Material { get; set; }
        public ObjectColor Color { get; set; }

        public BsShape(string id, ObjectTransform transform, ObjectLayer layer, bool simulated,
            Form form, ShapeMaterial material, ObjectColor color) : base(id, transform, layer, simulated)
        {
            Form = form;
            Material = material;
            Color = color;
        }

        public BsShape()
        {
        }

        public BsShape AttachAddon(BsAddon addon)
        {
            Addons.Add(addon);
            return this;
        }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<ShapeController>();
            controller.Object = this;

            // Convert path to mesh
            var points = Form.ToFillPoints(Transform.Rotation);
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
            meshRenderer.material = Color.Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            meshRenderer.material.color = Color.Color;
            meshRenderer.sortingOrder = MapSystem.Layer2Order(Layer);

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

            // Apply position
            gameObject.transform.position = Transform.Position;

            return controller;
        }

        protected override string[] _ToLcs()
        {
            return new[]
            {
                LcsParser.Stringify(Transform),
                LcsParser.Stringify(Form),
                LcsParser.Stringify(Material),
                LcsParser.Stringify(Color),
                LcsParser.Stringify(Layer),
                LcsParser.Stringify(Simulated)
            };
        }

        protected override void _FromLcs(string[] properties)
        {
            Transform = LcsParser.ParseTransform(properties[0]);
            Form = LcsParser.ParseForm(properties[1]);
            Material = LcsParser.ParseMaterial(properties[2]);
            Color = LcsParser.ParseColor(properties[3]);
            Layer = LcsParser.ParseLayer(properties[4]);
            Simulated = LcsParser.ParseBool(properties[5]);
        }
    }
}
