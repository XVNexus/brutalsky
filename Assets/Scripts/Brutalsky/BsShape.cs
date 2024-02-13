using System.Linq;
using Core;
using UnityEngine;
using Utils;

namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public BsPath path { get; set; }
        public BsMaterial material { get; set; }
        public BsColor color { get; set; }

        public BsShape(string id, BsPath path, BsMaterial material, BsColor color) : base(id)
        {
            this.path = path;
            this.material = material;
            this.color = color;
        }

        protected override GameObject _Create()
        {
            // Convert path to mesh
            var points = path.ToPoints();
            var vertices = points.Select(point => (Vector3)point).ToArray();
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = new Triangulator(points).Triangulate()
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Create new object and apply mesh
            var result = Instantiate(PrefabSystem.current.shape);
            result.GetComponent<MeshFilter>().mesh = mesh;
            var polygonCollider = result.GetComponent<PolygonCollider2D>();
            polygonCollider.SetPath(0, points);

            // Apply color
            var meshRenderer = result.GetComponent<MeshRenderer>();
            meshRenderer.sortingOrder = color.sortingOrder;
            meshRenderer.material.color = color.tint;

            // Apply material
            var physicsMaterial = new PhysicsMaterial2D
            {
                friction = material.friction,
                bounciness = material.restitution
            };
            polygonCollider.sharedMaterial = physicsMaterial;
            var rigidbody = result.GetComponent<Rigidbody2D>();
            rigidbody.sharedMaterial = physicsMaterial;
            if (!material.dynamic) return result;
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            polygonCollider.density = material.density;

            return result;
        }
    }
}
