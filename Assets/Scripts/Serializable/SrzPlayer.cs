using Brutalsky;
using Brutalsky.Object;

namespace Serializable
{
    public class SrzPlayer
    {
        public string id { get; set; }
        public float health { get; set; }
        public string color { get; set; }
        public bool dummy { get; set; }

        public static SrzPlayer Simplify(BsPlayer player)
        {
            return new SrzPlayer
            {
                id = player.id,
                health = player.health,
                color = player.color.ToString(),
                dummy = player.dummy
            };
        }

        public BsPlayer Expand()
        {
            return new BsPlayer
            (
                id,
                health,
                BsColor.Parse(color),
                dummy
            );
        }
    }
}
