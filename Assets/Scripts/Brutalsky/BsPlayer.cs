using Brutalsky.Object;
using Utils.Ext;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public string Name { get; set; }
        public float Health { get; set; }
        public BsColor Color { get; set; }
        public bool Dummy { get; set; }

        public BsPlayer(string id, string name, float health, BsColor color, bool dummy = false) : base(id, new BsTransform())
        {
            Name = name;
            Health = health;
            Color = color;
            Dummy = dummy;
        }

        public static float CalculateDamage(float impactForce)
        {
            return MathfExt.TMP(impactForce, 20f, .5f);
        }
    }
}
