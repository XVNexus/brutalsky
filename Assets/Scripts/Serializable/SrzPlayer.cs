using Brutalsky;
using Utils.Ext;
using Utils.Object;

namespace Serializable
{
    public class SrzPlayer
    {
        public string id { get; set; }
        public string nm { get; set; }
        public float hl { get; set; }
        public string cl { get; set; }
        public string dm { get; set; }

        public static SrzPlayer Simplify(BsPlayer player)
        {
            return new SrzPlayer
            {
                id = player.Id,
                nm = player.Name,
                hl = player.Health,
                cl = player.Color.ToString(),
                dm = BoolExt.ToString(player.Dummy)
            };
        }

        public BsPlayer Expand()
        {
            return new BsPlayer
            (
                id,
                nm,
                hl,
                ObjectColor.Parse(cl),
                BoolExt.Parse(dm)
            );
        }
    }
}
