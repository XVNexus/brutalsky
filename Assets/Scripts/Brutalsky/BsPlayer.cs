using Brutalsky.Object;
using Utils;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public float health { get; set; }
        public BsColor color { get; set; }
        public bool dummy { get; set; }

        public BsPlayer(string id, float health, BsColor color, bool dummy = false) : base(id, new BsTransform())
        {
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
