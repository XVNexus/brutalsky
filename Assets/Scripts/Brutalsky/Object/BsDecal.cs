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

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(Position),
                new(Rotation),
                new(Layer),
                new(Path),
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
            Color = (Color)props[i++].Value;
            Glow = (bool)props[i++].Value;
        }
    }
}
