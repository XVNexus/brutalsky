using UnityEngine;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public Vector2 size { get; set; }
        public BsChemical chemical { get; set; }
        public BsColor color { get; set; }
        public bool simulated { get; set; }

        public BsPool(BsTransform transform, Vector2 size, BsChemical chemical, BsColor color, bool simulated = true)
        {
            this.transform = transform;
            this.size = size;
            this.chemical = chemical;
            this.color = color;
            this.simulated = simulated;
        }
    }
}
