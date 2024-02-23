using Brutalsky;
using Brutalsky.Object;

namespace Serializable
{
    public class SrzPlayer
    {
        public string id { get; set; }
        public float health { get; set; }
        public BsColor color { get; set; }
        public bool dummy { get; set; }

        public static SrzPlayer Simplify(BsPlayer player)
        {
            return new SrzPlayer
            {
                id = player.id,
                health = player.health,
                color = player.color,
                dummy = player.dummy
            };
        }

        public BsPlayer Expand()
        {
            return new BsPlayer
            (
                id,
                health,
                color,
                dummy
            );
        }
    }
}
