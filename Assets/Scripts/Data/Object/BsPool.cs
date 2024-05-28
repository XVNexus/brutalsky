using Controllers;
using Controllers.Base;
using Data.Base;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsPool : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPool;
        public override string Tag => Tags.PoolPrefix;

        public Vector2 Size { get; set; } = Vector2.zero;
        public float Buoyancy { get; set; }
        public float Viscosity { get; set; }
        public float Health { get; set; }
        public Color Color { get; set; } = Color.white;
        public bool Glow { get; set; }
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

        public BsPool(string id = "", params string[] relatives) : base(id, relatives) { }

        protected override BsBehavior _Init(GameObject gameObject, BsObject[] relatedObjects)
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
    }
}
