using Brutalsky.Object;
using Brutalsky.Pool;
using UnityEngine;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public Vector2 Size { get; set; }
        public BsChemical Chemical { get; set; }
        public BsColor Color { get; set; }
        public BsLayer Layer { get; set; }
        public bool Simulated { get; set; }

        public BsPool(string id, BsTransform transform, Vector2 size, BsChemical chemical, BsColor color,
            BsLayer layer = BsLayer.Midground, bool simulated = true) : base(id, transform)
        {
            Size = size;
            Chemical = chemical;
            Color = color;
            Layer = layer;
            Simulated = simulated;
        }
    }
}
