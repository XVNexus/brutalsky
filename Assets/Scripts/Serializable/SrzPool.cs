using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Pool;
using UnityEngine;
using Utils;

namespace Serializable
{
    public class SrzPool
    {
        public string id { get; set; }
        public string transform { get; set; }
        public string size { get; set; }
        public string chemical { get; set; }
        public string color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public static SrzPool Simplify(BsPool pool)
        {
            return new SrzPool
            {
                id = pool.id,
                transform = pool.transform.ToString(),
                size = $"{pool.size.x} {pool.size.y}",
                chemical = pool.chemical.ToString(),
                color = pool.color.ToString(),
                layer = pool.layer,
                simulated = pool.simulated
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
                layer,
                simulated
            );
        }
    }
}
