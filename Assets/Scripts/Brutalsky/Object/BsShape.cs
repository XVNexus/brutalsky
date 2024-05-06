using System.Linq;
using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Lcs;
using Utils.Object;
using Utils.Shape;

namespace Brutalsky.Object
{
    public class BsShape : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pShape;
        public override string Tag => Tags.ShapePrefix;
        public override bool HasLogic => false;

        public Form Form { get; set; }
        public ShapeMaterial Material { get; set; }
        public bool Dynamic { get; set; }
        public Color Color { get; set; }
        public bool Glow { get; set; }

        public BsShape(string id, ObjectTransform transform, ObjectLayer layer, bool simulated, Form form,
            [CanBeNull] ShapeMaterial material = null, Color? color = null, bool glow = false)
            : base(id, transform, layer, simulated)
        {
            Form = form;
            Material = material ?? ShapeMaterial.Metal;
            Color = color ?? ColorExt.Ether;
            Glow = glow;
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
            meshRenderer.material = Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            meshRenderer.material.color = Color;
            meshRenderer.sortingOrder = MapSystem.LayerToOrder(Layer);

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
                if (Dynamic)
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
            gameObject.transform.localPosition = Transform.Position;

            return controller;
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(LcsType.Transform, Transform),
                new(LcsType.Form, Form),
                new(LcsType.Material, Material),
                new(LcsType.Bool, Dynamic),
                new(LcsType.Color, Color),
                new(LcsType.Bool, Glow),
                new(LcsType.Layer, Layer),
                new(LcsType.Bool, Simulated)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            Transform = (ObjectTransform)props[0].Value;
            Form = (Form)props[1].Value;
            Material = (ShapeMaterial)props[2].Value;
            Dynamic = (bool)props[3].Value;
            Color = (Color)props[4].Value;
            Glow = (bool)props[5].Value;
            Layer = (ObjectLayer)props[6].Value;
            Simulated = (bool)props[7].Value;
        }
    }
}
