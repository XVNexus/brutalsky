using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Shape;

namespace Serializable
{
    public class SrzShape
    {
        public string id { get; set; }
        public string transform { get; set; }
        public string path { get; set; }
        public string material { get; set; }
        public string color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public static SrzShape Simplify(BsShape shape)
        {
            return new SrzShape
            {
                id = shape.id,
                transform = shape.transform.ToString(),
                path = shape.path.ToString(),
                material = shape.material.ToString(),
                color = shape.color.ToString(),
                layer = shape.layer,
                simulated = shape.simulated
            };
        }

        public BsShape Expand()
        {
            return new BsShape
            (
                id,
                BsTransform.Parse(transform), 
                BsPath.Parse(path),
                BsMaterial.Parse(material), 
                BsColor.Parse(color),
                layer,
                simulated
            );
        }
    }
}
