using UnityEngine;
using Utils;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public string name { get; set; }
        public float health { get; set; }
        public Color color { get; set; }
        public bool dummy { get; set; }

        public override char saveSymbol => 'X';

        public BsPlayer(string name, float health, Color color, bool dummy = false)
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

        public override void Parse(string[][] raw)
        {
            name = raw[0][0];
            health = float.Parse(raw[1][0]);
            color = new Color(float.Parse(raw[2][0]), float.Parse(raw[2][1]), float.Parse(raw[2][2]));
            dummy = raw[3][0] == "1";
        }

        public override string[][] Stringify()
        {
            return new[]
            {
                new[] { name },
                new[] { Mathf.RoundToInt(health).ToString() },
                new[] { color.r.ToString(), color.g.ToString(), color.b.ToString() },
                new[] { dummy ? "1" : "0" }
            };
        }
    }
}
