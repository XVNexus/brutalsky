using Brutalsky;
using Brutalsky.Object;

namespace Serializable
{
    public class SrzPlayer
    {
        public string id { get; set; }
        public float health { get; set; }
        public string color { get; set; }
        public string dummy { get; set; }

        public static SrzPlayer Simplify(BsPlayer player)
        {
            return new SrzPlayer
            {
                id = player.id,
                health = player.health,
                color = player.color.ToString(),
                dummy = player.dummy ? "1" : "0"
            };
        }

        public BsPlayer Expand()
        {
            return new BsPlayer
            (
                id,
                health,
                BsColor.Parse(color),
                dummy == "1"
            );
        }
    }
}
