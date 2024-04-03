using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Shape;
using Utils.Ext;

namespace Serializable
{
    public class SrzShape
    {
        public string id { get; set; }
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
                id = shape.Id,
                tr = shape.Transform.ToString(),
                pt = shape.Path.ToString(),
                mt = shape.Material.ToString(),
                cl = shape.Color.ToString(),
                ly = (int)shape.Layer,
                sm = BoolExt.ToString(shape.Simulated)
            };
        }

        public BsShape Expand()
        {
            return new BsShape
            (
                id,
                BsTransform.Parse(tr), 
                BsPath.Parse(pt),
                BsMaterial.Parse(mt), 
                BsColor.Parse(cl),
                (BsLayer)ly,
                BoolExt.Parse(sm)
            );
        }
    }
}
