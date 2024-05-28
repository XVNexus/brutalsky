using System.Linq;
using Controllers;
using Controllers.Base;
using Data.Base;
using Systems;
using UnityEngine;
using Utils;
using Utils.Constants;

namespace Data.Object
{
    public class BsDecal : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pDecal;
        public override string Tag => Tags.DecalPrefix;

        public Path Path { get; set; }
        public Color Color { get; set; } = Color.white;
        public bool Glow { get; set; }

        public BsDecal(string id = "") : base(id) { }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<DecalController>();
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

            // Apply color and layer
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            meshRenderer.material.color = Color;
            meshRenderer.sortingOrder = Layer * 2;

            // Apply transform
            gameObject.transform.localPosition = Position;

            return controller;
        }
    }
}
