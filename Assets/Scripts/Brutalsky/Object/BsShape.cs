using System.Linq;
using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Lcs;
using Utils.Path;

namespace Brutalsky.Object
{
    public class BsShape : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pShape;
        public override string Tag => Tags.ShapePrefix;

        public Vector2 Position { get; set; } = Vector2.zero;
        public float Rotation { get; set; }
        public sbyte Layer { get; set; }
        public PathString Path { get; set; }
        public (float, float, float, float, float) Material
        {
            get => (Friction, Restitution, Adhesion, Density, Health);
            set
            {
                Friction = value.Item1;
                Restitution = value.Item2;
                Adhesion = value.Item3;
                Density = value.Item4;
                Health = value.Item5;
            }
        }
        public float Friction { get; set; }
        public float Restitution { get; set; }
        public float Adhesion { get; set; }
        public float Density { get; set; }
        public float Health { get; set; }
        public bool Dynamic { get; set; }
        public Color Color { get; set; } = Color.white;
        public bool Glow { get; set; }

        public BsShape(string id = "") : base(id) { }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<ShapeController>();
            controller.Object = this;

            // Convert path to mesh
            var points = Path.ToPoints(Rotation);
            var vertices = points.Select(point => (Vector3)point).ToArray();
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new PathTriangulator(points).Triangulate()
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
            meshRenderer.sortingOrder = Layer * 2;

            // Apply material
            var rigidbody = gameObject.GetComponent<Rigidbody2D>();
            var physicsMaterial = new PhysicsMaterial2D
            {
                friction = Friction,
                bounciness = Restitution
            };
            polygonCollider.sharedMaterial = physicsMaterial;
            rigidbody.sharedMaterial = physicsMaterial;
            if (Dynamic)
            {
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                polygonCollider.density = Density;
            }
            if (ParentTag == Tags.ShapePrefix && ParentId.Length > 0)
            {
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }

            // Apply transform
            gameObject.transform.localPosition = Position;

            return controller;
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(Position),
                new(Rotation),
                new(Layer),
                new(Path),
                new(Friction),
                new(Restitution),
                new(Adhesion),
                new(Density),
                new(Health),
                new(Dynamic),
                new(Color),
                new(Glow)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++].Value;
            Rotation = (float)props[i++].Value;
            Layer = (sbyte)props[i++].Value;
            Path = (PathString)props[i++].Value;
            Friction = (float)props[i++].Value;
            Restitution = (float)props[i++].Value;
            Adhesion = (float)props[i++].Value;
            Density = (float)props[i++].Value;
            Health = (float)props[i++].Value;
            Dynamic = (bool)props[i++].Value;
            Color = (Color)props[i++].Value;
            Glow = (bool)props[i++].Value;
        }
    }
}
