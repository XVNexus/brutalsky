using Brutalsky;
using Brutalsky.Object;

namespace Serializable
{
    public class SrzSpawn
    {
        public string id { get; set; }
        public string tr { get; set; }
        public int pr { get; set; }

        public static SrzSpawn Simplify(BsSpawn spawn)
        {
            return new SrzSpawn
            {
                id = spawn.Id,
                tr = spawn.Transform.ToString(),
                pr = spawn.Priority
            };
        }

        public BsSpawn Expand()
        {
            return new BsSpawn
            (
                id,
                BsTransform.Parse(tr),
                pr
            );
        }
    }
}
