using System.Collections.Generic;
using Brutalsky.Base;
using Controllers;
using Core;
using UnityEngine;
using Utils;
using Utils.Constants;
using Utils.Ext;
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
            // Link object to controller
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

        protected override string[] _ToSrz()
        {
            return new[]
            {
                Transform.ToString(),
                Vector2Ext.Stringify(Size),
                Chemical.ToString(),
                Color.ToString(),
                ((int)Layer).ToString(),
                BoolExt.Stringify(Simulated)
            };
        }

        protected override void _FromSrz(string[] properties)
        {
            Transform = ObjectTransform.Parse(properties[0]);
            Size = Vector2Ext.Parse(properties[1]);
            Chemical = PoolChemical.Parse(properties[2]);
            Color = ObjectColor.Parse(properties[3]);
            Layer = (ObjectLayer)int.Parse(properties[4]);
            Simulated = BoolExt.Parse(properties[5]);
        }
    }
}
