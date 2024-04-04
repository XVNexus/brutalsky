using Brutalsky;
using Utils.Ext;

namespace Serializable
{
    public class SrzSpawn
    {
        public string ps { get; set; }
        public int pr { get; set; }

        public static SrzSpawn Simplify(BsSpawn spawn)
        {
            return new SrzSpawn
            {
                ps = Vector2Ext.ToString(spawn.Position),
                pr = spawn.Priority
            };
        }

        public BsSpawn Expand()
        {
            return new BsSpawn
            (
                Vector2Ext.Parse(ps),
                pr
            );
        }
    }
}
