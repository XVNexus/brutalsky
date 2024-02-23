using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Shape;

namespace Serializable
{
    public class SrzShape
    {
        public string id { get; set; }
        public BsTransform transform { get; set; }
        public string path { get; set; }
        public BsMaterial material { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public static SrzShape Simplify(BsShape shape)
        {
            return new SrzShape
            {
                id = shape.id,
                transform = shape.transform,
                path = shape.path.ToString(),
                material = shape.material,
                color = shape.color,
                layer = shape.layer,
                simulated = shape.simulated
            };
        }

        public BsShape Expand()
        {
            return new BsShape
            (
                id,
                transform,
                BsPath.FromString(path),
                material,
                color,
                layer,
                simulated
            );
        }
    }
}
