using System;
using Controllers;
using Controllers.Base;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsPool : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPool;
        public override string Tag => Tags.PoolPrefix;

        public override Func<GameObject, BsObject, BsObject[], BsBehavior> Init => (gob, obj, _) =>
        {
            // Link object to controller
            var pool = obj.As<BsPool>();
            var controller = gob.GetComponent<PoolController>();
            controller.Object = pool;

            // Apply size
            gob.transform.localScale = pool.Size;

            // Apply color and layer
            var spriteRenderer = gob.GetComponent<SpriteRenderer>();
            spriteRenderer.material = pool.Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            spriteRenderer.material.color = pool.Color;
            spriteRenderer.sortingOrder = pool.Layer * 2;

            // Apply transform
            gob.transform.localPosition = pool.Position;
            gob.transform.localRotation = Quaternion.Euler(0f, 0f, pool.Rotation);

            return controller;
        };

        public Vector2 Size
        {
            get => Vector2Ext.FromLcs(Props[0]);
            set => Props[0] = value.ToLcs();
        }

        public float Buoyancy
        {
            get => (float)Props[1];
            set => Props[1] = value;
        }

        public float Viscosity
        {
            get => (float)Props[2];
            set => Props[2] = value;
        }

        public float Health
        {
            get => (float)Props[3];
            set => Props[3] = value;
        }

        public Color Color
        {
            get => ColorExt.FromLcs(Props[4]);
            set => Props[4] = value.ToLcs();
        }

        public bool Glow
        {
            get => (bool)Props[5];
            set => Props[5] = value;
        }

        public (float, float, float) Chemical
        {
            get => (Buoyancy, Viscosity, Health);
            set
            {
                Buoyancy = value.Item1;
                Viscosity = value.Item2;
                Health = value.Item3;
            }
        }

        public BsPool(string id = "", params string[] relatives) : base(id, relatives)
        {
            Props = new[] { Vector2.zero.ToLcs(), 0f, 0f, 0f, Color.white.ToLcs(), false };
        }

        public BsPool() { }
    }
}
