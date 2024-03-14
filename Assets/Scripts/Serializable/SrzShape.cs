using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Shape;
using Utils.Ext;

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
                id = shape.Id,
                transform = shape.Transform.ToString(),
                path = shape.Path.ToString(),
                material = shape.Material.ToString(),
                color = shape.Color.ToString(),
                layer = (int)shape.Layer,
                simulated = BoolExt.ToString(shape.Simulated)
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
