using Brutalsky.Property;
using UnityEngine;
using Utils;

namespace Brutalsky.Object
{
    public class BsPlayer : BsObject
    {
        public string name { get; set; }
        public float health { get; set; }
        public BsColor color { get; set; }
        public bool dummy { get; set; }

        public BsPlayer(string name, float health, BsColor color, bool dummy = false)
        {
            this.name = name;
            this.health = health;
            this.color = color;
            this.dummy = dummy;
        }

        public BsPlayer()
        {
        }

        public static float CalculateDamage(float impactForce)
        {
            return MathfExt.TMP(impactForce, 20f, .5f);
        }

        public override void Parse(string[] raw)
        {
            name = Sanitizer.Desanitize(raw[0]);
            health = float.Parse(raw[1]);
            color = new BsColor();
            color.Parse(raw[2]);
            dummy = raw[3] == "1";
        }

        public override string[] Stringify()
        {
            return new[]
            {
                Sanitizer.Sanitize(name),
                Mathf.RoundToInt(health).ToString(),
                color.Stringify(),
                dummy ? "1" : "0"
            };
        }
    }
}
