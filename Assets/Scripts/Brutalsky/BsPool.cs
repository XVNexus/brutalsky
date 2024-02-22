using Brutalsky.Object;
using Brutalsky.Pool;
using UnityEngine;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public Vector2 size { get; set; }
        public BsChemical chemical { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public BsPool(string id, BsTransform transform, Vector2 size, BsChemical chemical, BsColor color,
            BsLayer layer = BsLayer.Midground, bool simulated = true) : base(id)
        {
            this.transform = transform;
            this.size = size;
            this.chemical = chemical;
            this.color = color;
            this.layer = layer;
            this.simulated = simulated;
        }
    }
}
