using Brutalsky;
using Utils.Ext;
using Utils.Object;
using Utils.Path;
using Utils.Shape;

namespace Serializable
{
    public class SrzShape
    {
        public string tr { get; set; }
        public string pt { get; set; }
        public string mt { get; set; }
        public string cl { get; set; }
        public int ly { get; set; }
        public string sm { get; set; }

        public static SrzShape Simplify(BsShape shape)
        {
            return new SrzShape
            {
                tr = shape.Transform.ToString(),
                pt = shape.Path.ToString(),
                mt = shape.Material.ToString(),
                cl = shape.Color.ToString(),
                ly = (int)shape.Layer,
                sm = BoolExt.Stringify(shape.Simulated)
            };
        }

        public BsShape Expand(string id)
        {
            return new BsShape
            (
                id,
                ObjectTransform.Parse(tr), 
                Path.Parse(pt),
                ShapeMaterial.Parse(mt), 
                ObjectColor.Parse(cl),
                (ObjectLayer)ly,
                BoolExt.Parse(sm)
            );
        }
    }
}
