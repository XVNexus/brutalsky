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
                Stringifier.GetString(Transform),
                Stringifier.GetString(Size),
                Stringifier.GetString(Chemical),
                Stringifier.GetString(Color),
                Stringifier.GetString(Glow),
                Stringifier.GetString(Layer),
                Stringifier.GetString(Simulated)
            };
        }

        protected override void _FromLcs(string[] properties)
        {
            Transform = Stringifier.ToTransform(properties[0]);
            Size = Stringifier.ToVector2(properties[1]);
            Chemical = Stringifier.ToChemical(properties[2]);
            Color = Stringifier.ToColor(properties[3]);
            Glow = Stringifier.ToBoolean(properties[4]);
            Layer = Stringifier.ToLayer(properties[5]);
            Simulated = Stringifier.ToBoolean(properties[6]);
        }
    }
}
