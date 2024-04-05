using System.Collections.Generic;
using Brutalsky.Base;
using Controllers;
using Core;
using UnityEngine;
using Utils;
using Utils.Constants;
using Utils.Object;
using Utils.Pool;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.poolPrefab;
        public override string Tag => Tags.Pool;
        public Vector2 Size { get; set; }
        public PoolChemical Chemical { get; set; }
        public ObjectColor Color { get; set; }
        public ObjectLayer Layer { get; set; }
        public bool Simulated { get; set; }

        public BsPool(string id, ObjectTransform transform, Vector2 size, PoolChemical chemical, ObjectColor color,
            ObjectLayer layer = ObjectLayer.Midground, bool simulated = true) : base(id, transform)
        {
            Size = size;
            Chemical = chemical;
            Color = color;
            Layer = layer;
            Simulated = simulated;
        }

        public BsPool()
        {
        }

        protected override Component _Init(GameObject gameObject, BsMap map)
        {
            // Create new object
            var controller = gameObject.GetComponent<PoolController>();
            controller.Object = this;

            // Apply size
            gameObject.transform.localScale = Size;

            // Apply color and layer
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = Color.Glow ? ResourceSystem._.unlitMaterial : ResourceSystem._.litMaterial;
            spriteRenderer.material.color = Color.Tint;
            spriteRenderer.sortingOrder = BsUtils.Layer2Order(Layer);

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

        protected override Dictionary<string, string> _ToSrz()
        {
            throw new System.NotImplementedException();
        }

        protected override void _FromSrz(Dictionary<string, string> properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
