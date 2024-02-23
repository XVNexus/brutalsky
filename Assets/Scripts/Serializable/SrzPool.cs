using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Pool;
using UnityEngine;

namespace Serializable
{
    public class SrzPool
    {
        public string id { get; set; }
        public BsTransform transform { get; set; }
        public Vector2 size { get; set; }
        public BsChemical chemical { get; set; }
        public BsColor color { get; set; }
        public BsLayer layer { get; set; }
        public bool simulated { get; set; }

        public static SrzPool Simplify(BsPool pool)
        {
            return new SrzPool
            {
                id = pool.id,
                transform = pool.transform,
                size = pool.size,
                chemical = pool.chemical,
                color = pool.color,
                layer = pool.layer,
                simulated = pool.simulated
            };
        }

        public BsPool Expand()
        {
            return new BsPool
            (
                id,
                transform,
                size,
                chemical,
                color,
                layer,
                simulated
            );
        }
    }
}
