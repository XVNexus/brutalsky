using Brutalsky;
using Brutalsky.Object;

namespace Serializable
{
    public class SrzSpawn
    {
        public string id { get; set; }
        public string transform { get; set; }
        public int priority { get; set; }

        public static SrzSpawn Simplify(BsSpawn spawn)
        {
            return new SrzSpawn
            {
                id = spawn.id,
                transform = spawn.transform.ToString(),
                priority = spawn.priority
            };
        }

        public BsSpawn Expand()
        {
            return new BsSpawn
            (
                id,
                BsTransform.Parse(transform),
                priority
            );
        }
    }
}
