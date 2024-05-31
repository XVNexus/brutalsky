using System;
using System.Linq;
using Controllers;
using Controllers.Base;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsShape : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pShape;
        public override string Tag => Tags.ShapePrefix;

        public override Func<GameObject, BsObject, BsObject[], BsBehavior> Init => (gob, obj, _) =>
        {
            // Link object to controller
            var shape = obj.As<BsShape>();
            var controller = gob.GetComponent<ShapeController>();
            controller.Object = shape;

            // Convert path to mesh
            var points = shape.Path.GetPoints(shape.Rotation);
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
            var polygonCollider = gob.GetComponent<PolygonCollider2D>();
            polygonCollider.SetPath(0, points);

            // Apply color and layer
            var meshRenderer = gob.GetComponent<MeshRenderer>();
            meshRenderer.material = shape.Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            meshRenderer.material.color = shape.Color;
            meshRenderer.sortingOrder = shape.Layer * 2;

            // Apply material
            var rigidbody = gob.GetComponent<Rigidbody2D>();
            var physicsMaterial = new PhysicsMaterial2D
            {
                friction = shape.Friction,
                bounciness = shape.Restitution
            };
            polygonCollider.sharedMaterial = physicsMaterial;
            rigidbody.sharedMaterial = physicsMaterial;
            if (shape.Dynamic)
            {
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                polygonCollider.density = shape.Density;
            }

            if (shape.Relatives.Length > 0)
            {
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }

            // Apply transform
            gob.transform.localPosition = shape.Position;

            return controller;
        };

        public Path Path
        {
            get => Path.FromLcs(Props[0]);
            set => Props[0] = value.ToLcs();
        }

        public float Friction
        {
            get => (float)Props[1];
            set => Props[1] = value;
        }

        public float Restitution
        {
            get => (float)Props[2];
            set => Props[2] = value;
        }

        public float Adhesion
        {
            get => (float)Props[3];
            set => Props[3] = value;
        }

        public float Density
        {
            get => (float)Props[4];
            set => Props[4] = value;
        }

        public float Health
        {
            get => (float)Props[5];
            set => Props[5] = value;
        }

        public bool Dynamic
        {
            get => (bool)Props[6];
            set => Props[6] = value;
        }

        public Color Color
        {
            get => ColorExt.FromLcs(Props[7]);
            set => Props[7] = value.ToLcs();
        }

        public bool Glow
        {
            get => (bool)Props[8];
            set => Props[8] = value;
        }

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

        public BsShape(string id = "", params string[] relatives) : base(id, relatives)
        {
            Props = new[]
            {
                Path.Ngon(3, 1f).ToLcs(), 0f, 0f, 0f, 0f, 0f, false, Color.white.ToLcs(), false
            };
        }

        public BsShape() { }
    }
}
