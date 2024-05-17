using System.Linq;
using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Path;

namespace Brutalsky.Object
{
    public class BsDecal : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pDecal;
        public override string Tag => Tags.DecalPrefix;

        public Vector2 Position { get; set; } = Vector2.zero;
        public float Rotation { get; set; }
        public sbyte Layer { get; set; }
        public PathString Path { get; set; }
        public Color Color { get; set; } = Color.white;
        public bool Glow { get; set; }

        public BsDecal(string id = "") : base(id) { }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<DecalController>();
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

            // Apply color and layer
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            meshRenderer.material.color = Color;
            meshRenderer.sortingOrder = Layer * 2;

            // Apply transform
            gameObject.transform.localPosition = Position;

            return controller;
        }

        protected override object[] _ToLcs()
        {
            return new object[] { Position, Rotation, Layer, Path, Color, Glow };
        }

        protected override void _FromLcs(object[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++];
            Rotation = (float)props[i++];
            Layer = (sbyte)props[i++];
            Path = (PathString)props[i++];
            Color = (Color)props[i++];
            Glow = (bool)props[i++];
        }
    }
}
