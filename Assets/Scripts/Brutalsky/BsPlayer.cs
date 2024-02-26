using Brutalsky.Object;
using Utils.Ext;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public string name { get; set; }
        public float health { get; set; }
        public BsColor color { get; set; }
        public bool dummy { get; set; }

        public BsPlayer(string id, string name, float health, BsColor color, bool dummy = false) : base(id, new BsTransform())
        {
            this.name = name;
            this.health = health;
            this.color = color;
            this.dummy = dummy;
        }

        public static float CalculateDamage(float impactForce)
        {
            return MathfExt.TMP(impactForce, 20f, .5f);
        }
    }
}
