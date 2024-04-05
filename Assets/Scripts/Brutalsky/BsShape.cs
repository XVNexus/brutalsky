using Brutalsky.Base;
using Utils.Object;
using Utils.Path;
using Utils.Shape;

namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public Path Path { get; set; }
        public ShapeMaterial Material { get; set; }
        public ObjectColor Color { get; set; }
        public ObjectLayer Layer { get; set; }
        public bool Simulated { get; set; }

        public BsShape(string id, ObjectTransform transform, Path path, ShapeMaterial material, ObjectColor color,
            ObjectLayer layer = ObjectLayer.Midground, bool simulated = true) : base(id, transform)
        {
            Path = path;
            Material = material;
            Color = color;
            Layer = layer;
            Simulated = simulated;
        }
    }
}
