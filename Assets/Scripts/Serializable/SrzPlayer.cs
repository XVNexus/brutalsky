using Brutalsky;
using Brutalsky.Object;
using Utils;

namespace Serializable
{
    public class SrzPlayer
    {
        public string id { get; set; }
        public string name { get; set; }
        public float health { get; set; }
        public string color { get; set; }
        public string dummy { get; set; }

        public static SrzPlayer Simplify(BsPlayer player)
        {
            return new SrzPlayer
            {
                id = player.id,
                name = player.name,
                health = player.health,
                color = player.color.ToString(),
                dummy = BoolExt.ToString(player.dummy)
            };
        }

        public BsPlayer Expand()
        {
            return new BsPlayer
            (
                id,
                name,
                health,
                BsColor.Parse(color),
                BoolExt.Parse(dummy)
            );
        }
    }
}
