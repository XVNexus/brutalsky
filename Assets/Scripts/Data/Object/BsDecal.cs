using System.Linq;
using Controllers;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsDecal : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pDecal;
        public override string Tag => Tags.DecalPrefix;

        public Path Path
        {
            get => Path.FromLcs(Props[0]);
            set => Props[0] = value.ToLcs();
        }

        public Color Color
        {
            get => ColorExt.FromLcs(Props[1]);
            set => Props[1] = value.ToLcs();
        }

        public bool Glow
        {
            get => (bool)Props[2];
            set => Props[2] = value;
        }

        public BsDecal(string id = "", params string[] relatives) : base(id, relatives)
        {
            Props = new[] { Path.Ngon(3, 1f).ToLcs(), Color.white.ToLcs(), false };
            Init = (gob, obj, _) =>
            {
                // Link object to controller
                var decal = obj.As<BsDecal>();
                var controller = gob.GetComponent<DecalController>();
                controller.Object = decal;

                // Convert path to mesh
                var points = decal.Path.GetPoints(decal.Rotation);
                var vertices = points.Select(point => (Vector3)point).ToArray();
                var mesh = new Mesh
                {
                    vertices = vertices,
                    triangles = new Triangulator(points).Triangulate()
                };
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                // Apply mesh
                gob.GetComponent<MeshFilter>().mesh = mesh;

                // Apply color and layer
                var meshRenderer = gob.GetComponent<MeshRenderer>();
                meshRenderer.material = decal.Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
                meshRenderer.material.color = decal.Color;
                meshRenderer.sortingOrder = decal.Layer * 2;

                // Apply transform
                gob.transform.localPosition = decal.Position;

                return controller;
            };
        }

        public BsDecal() { }
    }
}
