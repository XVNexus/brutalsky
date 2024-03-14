using Brutalsky;
using Brutalsky.Object;
using Utils.Ext;

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
                id = player.Id,
                name = player.Name,
                health = player.Health,
                color = player.Color.ToString(),
                dummy = BoolExt.ToString(player.Dummy)
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
