using Brutalsky.Object;
using Brutalsky.Shape;
using UnityEngine;

namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public BsPath path { get; set; }
        public BsMaterial material { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public BsShape(string id, BsTransform transform, BsPath path, BsMaterial material, BsColor color,
            BsLayer layer = BsLayer.Midground, bool simulated = true) : base(id, transform)
        {
            this.path = path;
            this.material = material;
            this.color = color;
            this.layer = layer;
            this.simulated = simulated;
        }
    }
}
