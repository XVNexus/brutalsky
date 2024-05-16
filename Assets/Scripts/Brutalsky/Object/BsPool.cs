using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Lcs;

namespace Brutalsky.Object
{
    public class BsPool : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPool;
        public override string Tag => Tags.PoolPrefix;

        public Vector2 Position { get; set; } = Vector2.zero;
        public float Rotation { get; set; }
        public sbyte Layer { get; set; }
        public Vector2 Size { get; set; } = Vector2.zero;
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
        public float Buoyancy { get; set; }
        public float Viscosity { get; set; }
        public float Health { get; set; }
        public Color Color { get; set; } = Color.white;
        public bool Glow { get; set; }

        public BsPool(string id = "") : base(id) { }

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
            spriteRenderer.sortingOrder = Layer * 2;

            // Apply transform
            gameObject.transform.localPosition = Position;
            gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, Rotation);

            return controller;
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(LcsType.Float2, Position),
                new(LcsType.Float, Rotation),
                new(LcsType.SByte, Layer),
                new(LcsType.Float2, Size),
                new(LcsType.Float, Buoyancy),
                new(LcsType.Float, Viscosity),
                new(LcsType.Float, Health),
                new(LcsType.Color, Color),
                new(LcsType.Bool, Glow)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++].Value;
            Rotation = (float)props[i++].Value;
            Layer = (sbyte)props[i++].Value;
            Size = (Vector2)props[i++].Value;
            Buoyancy = (float)props[i++].Value;
            Viscosity = (float)props[i++].Value;
            Health = (float)props[i++].Value;
            Color = (Color)props[i++].Value;
            Glow = (bool)props[i++].Value;
        }
    }
}
