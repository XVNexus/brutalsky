using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;

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

        protected override object[] _ToLcs()
        {
            return new object[] { Position, Rotation, Layer, Size, Buoyancy, Viscosity, Health, Color, Glow };
        }

        protected override void _FromLcs(object[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++];
            Rotation = (float)props[i++];
            Layer = (sbyte)props[i++];
            Size = (Vector2)props[i++];
            Buoyancy = (float)props[i++];
            Viscosity = (float)props[i++];
            Health = (float)props[i++];
            Color = (Color)props[i++];
            Glow = (bool)props[i++];
        }
    }
}
