using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Pool;
using Utils.Ext;

namespace Serializable
{
    public class SrzPool
    {
        public string id { get; set; }
        public string tr { get; set; }
        public string sz { get; set; }
        public string ch { get; set; }
        public string cl { get; set; }
        public int ly { get; set; }
        public string sm { get; set; }

        public static SrzPool Simplify(BsPool pool)
        {
            return new SrzPool
            {
                id = pool.Id,
                tr = pool.Transform.ToString(),
                sz = $"{pool.Size.x} {pool.Size.y}",
                ch = pool.Chemical.ToString(),
                cl = pool.Color.ToString(),
                ly = (int)pool.Layer,
                sm = BoolExt.ToString(pool.Simulated)
            };
        }

        public BsPool Expand()
        {
            return new BsPool
            (
                id,
                BsTransform.Parse(tr),
                Vector2Ext.Parse(sz),
                BsChemical.Parse(ch), 
                BsColor.Parse(cl),
                (BsLayer)ly,
                BoolExt.Parse(sm)
            );
        }
    }
}
