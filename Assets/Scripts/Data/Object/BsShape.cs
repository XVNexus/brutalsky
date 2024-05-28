using System.Linq;
using Controllers;
using Controllers.Base;
using Data.Base;
using Lcs;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsShape : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pShape;
        public override string Tag => Tags.ShapePrefix;

        public Path Path { get; set; }
        public float Friction { get; set; }
        public float Restitution { get; set; }
        public float Adhesion { get; set; }
        public float Density { get; set; }
        public float Health { get; set; }
        public bool Dynamic { get; set; }
        public Color Color { get; set; } = Color.white;
        public bool Glow { get; set; }
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

        public BsShape(string id = "", params string[] relatives) : base(id, relatives) { }

        protected override BsBehavior _Init(GameObject gameObject, BsObject[] relatedObjects)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<ShapeController>();
            controller.Object = this;

            // Convert path to mesh
            var points = Path.GetPoints(Rotation);
            var vertices = points.Select(point => (Vector3)point).ToArray();
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new Triangulator(points).Triangulate()
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
            if (Relatives.Length > 0)
            {
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }

            // Apply transform
            gameObject.transform.localPosition = Position;

            return controller;
        }
    }
}
