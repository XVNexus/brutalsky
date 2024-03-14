using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Pool;
using Utils.Ext;

namespace Serializable
{
    public class SrzPool
    {
        public string id { get; set; }
        public string transform { get; set; }
        public string size { get; set; }
        public string chemical { get; set; }
        public string color { get; set; }
        public int layer { get; set; }
        public string simulated { get; set; }

        public static SrzPool Simplify(BsPool pool)
        {
            return new SrzPool
            {
                id = pool.Id,
                transform = pool.Transform.ToString(),
                size = $"{pool.Size.x} {pool.Size.y}",
                chemical = pool.Chemical.ToString(),
                color = pool.Color.ToString(),
                layer = (int)pool.Layer,
                simulated = BoolExt.ToString(pool.Simulated)
            };
        }

        public BsPool Expand()
        {
            return new BsPool
            (
                id,
                BsTransform.Parse(transform),
                Vector2Ext.Parse(size),
                BsChemical.Parse(chemical), 
                BsColor.Parse(color),
                (BsLayer)layer,
                BoolExt.Parse(simulated)
            );
        }
    }
}
