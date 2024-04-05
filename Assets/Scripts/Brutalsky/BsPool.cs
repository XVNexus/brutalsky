using Brutalsky.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Object;
using Utils.Pool;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public override GameObject Prefab => MapSystem._.poolPrefab;
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
    }
}
