using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;
using Utils.Pool;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPool;
        public override char Tag => Tags.PoolSym;

        public Vector2 Size { get; set; }
        public PoolChemical Chemical { get; set; }
        public ObjectColor Color { get; set; }

        public BsPool(string id, ObjectTransform transform, Vector2 size, ObjectLayer layer, bool simulated,
            PoolChemical chemical, ObjectColor color) : base(id, transform, layer, simulated)
        {
            Size = size;
            Chemical = chemical;
            Color = color;
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
            spriteRenderer.material = Color.Glow ? ResourceSystem._.aUnlitMaterial : ResourceSystem._.aLitMaterial;
            spriteRenderer.material.color = Color.Color;
            spriteRenderer.sortingOrder = MapSystem.Layer2Order(Layer);

            // Apply chemical
            if (!Simulated)
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                controller.enabled = false;
            }

            // Apply position and rotation
            gameObject.transform.position = Transform.Position;
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Transform.Rotation);

            return controller;
        }

        protected override string[] _ToLcs()
        {
            return new[]
            {
                LcsParser.Stringify(Transform),
                LcsParser.Stringify(Size),
                LcsParser.Stringify(Chemical),
                LcsParser.Stringify(Color),
                LcsParser.Stringify(Layer),
                LcsParser.Stringify(Simulated)
            };
        }

        protected override void _FromLcs(string[] properties)
        {
            Transform = LcsParser.ParseTransform(properties[0]);
            Size = LcsParser.ParseVector2(properties[1]);
            Chemical = LcsParser.ParseChemical(properties[2]);
            Color = LcsParser.ParseColor(properties[3]);
            Layer = LcsParser.ParseLayer(properties[4]);
            Simulated = LcsParser.ParseBool(properties[5]);
        }
    }
}
