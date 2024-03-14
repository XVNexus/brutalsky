using Brutalsky.Object;
using Brutalsky.Shape;

namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public BsPath Path { get; set; }
        public BsMaterial Material { get; set; }
        public BsColor Color { get; set; }
        public BsLayer Layer { get; set; }
        public bool Simulated { get; set; }

        public BsShape(string id, BsTransform transform, BsPath path, BsMaterial material, BsColor color,
            BsLayer layer = BsLayer.Midground, bool simulated = true) : base(id, transform)
        {
            Path = path;
            Material = material;
            Color = color;
            Layer = layer;
            Simulated = simulated;
        }
    }
}
