using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Shape;
using Utils;

namespace Serializable
{
    public class SrzShape
    {
        public string id { get; set; }
        public string transform { get; set; }
        public string path { get; set; }
        public string material { get; set; }
        public string color { get; set; }
        public int layer { get; set; }
        public string simulated { get; set; }

        public static SrzShape Simplify(BsShape shape)
        {
            return new SrzShape
            {
                id = shape.id,
                transform = shape.transform.ToString(),
                path = shape.path.ToString(),
                material = shape.material.ToString(),
                color = shape.color.ToString(),
                layer = (int)shape.layer,
                simulated = BoolExt.ToString(shape.simulated)
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
                (BsLayer)layer,
                BoolExt.Parse(simulated)
            );
        }
    }
}
