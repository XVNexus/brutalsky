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
        public override string Tag => Tags.PoolLTag;
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

        protected override string[] _ToLcs()
        {
            return new[]
            {
                Stringifier.Str(LcsType.Transform, Transform),
                Stringifier.Str(LcsType.Vector2, Size),
                Stringifier.Str(LcsType.Chemical, Chemical),
                Stringifier.Str(LcsType.Color, Color),
                Stringifier.Str(LcsType.Bool, Glow),
                Stringifier.Str(LcsType.Layer, Layer),
                Stringifier.Str(LcsType.Bool, Simulated)
            };
        }

        protected override void _FromLcs(string[] properties)
        {
            Transform = Stringifier.Par<ObjectTransform>(LcsType.Transform, properties[0]);
            Size = Stringifier.Par<Vector2>(LcsType.Vector2, properties[1]);
            Chemical = Stringifier.Par<PoolChemical>(LcsType.Chemical, properties[2]);
            Color = Stringifier.Par<Color>(LcsType.Color, properties[3]);
            Glow = Stringifier.Par<bool>(LcsType.Bool, properties[4]);
            Layer = Stringifier.Par<ObjectLayer>(LcsType.Layer, properties[5]);
            Simulated = Stringifier.Par<bool>(LcsType.Bool, properties[6]);
        }
    }
}
