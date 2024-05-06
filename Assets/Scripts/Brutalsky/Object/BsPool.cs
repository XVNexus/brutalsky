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
using Utils.Pool;

namespace Brutalsky.Object
{
    public class BsPool : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPool;
        public override string Tag => Tags.PoolPrefix;
        public override bool HasLogic => false;

        public Vector2 Size { get; set; }
        public PoolChemical Chemical { get; set; }
        public Color Color { get; set; }
        public bool Glow { get; set; }

        public BsPool(string id, ObjectTransform transform, ObjectLayer layer, bool simulated, Vector2 size,
            [CanBeNull] PoolChemical chemical = null, Color? color = null, bool glow = false)
            : base(id, transform, layer, simulated)
        {
            Size = size;
            Chemical = chemical ?? PoolChemical.Water;
            Color = color ?? ColorExt.Ether;
            Glow = glow;
        }

        public BsPool()
        {
        }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<PoolController>();
            controller.Object = this;

            // Apply size
            gameObject.transform.localScale = Size;

            // Apply color and layer
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            spriteRenderer.material.color = Color;
            spriteRenderer.sortingOrder = MapSystem.LayerToOrder(Layer);

            // Apply chemical
            if (!Simulated)
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                controller.enabled = false;
            }

            // Apply position and rotation
            gameObject.transform.localPosition = Transform.Position;
            gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, Transform.Rotation);

            return controller;
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(LcsType.Transform, Transform),
                new(LcsType.Vector2, Size),
                new(LcsType.Chemical, Chemical),
                new(LcsType.Color, Color),
                new(LcsType.Bool, Glow),
                new(LcsType.Layer, Layer),
                new(LcsType.Bool, Simulated)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            Transform = (ObjectTransform)props[0].Value;
            Size = (Vector2)props[1].Value;
            Chemical = (PoolChemical)props[2].Value;
            Color = (Color)props[3].Value;
            Glow = (bool)props[4].Value;
            Layer = (ObjectLayer)props[5].Value;
            Simulated = (bool)props[6].Value;
        }
    }
}
