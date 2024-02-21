using UnityEngine;

namespace Brutalsky.Property
{
    public class BsColor : BsProperty
    {
        public Color tint { get; set; }
        public bool glow { get; set; }

        public BsColor(Color tint, bool glow = false)
        {
            this.tint = tint;
            this.glow = glow;
        }

        public BsColor(float r, float g, float b, float a = 1f, bool glow = false)
        {
            tint = new Color(r, g, b, a);
            this.glow = glow;
        }

        public BsColor()
        {
        }

        public static BsColor Wood()
            => new BsColor(new Color(.9f, .6f, .3f));

        public static BsColor Metal()
            => new BsColor(new Color(.9f, .9f, 1f));

        public static BsColor Stone()
            => new BsColor(new Color(.7f, .7f, .7f));

        public static BsColor Ice()
            => new BsColor(new Color(.5f, .8f, 1f));

        public static BsColor Rubber()
            => new BsColor(new Color(.7f, .4f, .9f));

        public static BsColor Glue()
            => new BsColor(new Color(.7f, .9f, .4f));

        public static BsColor Medkit()
            => new BsColor(new Color(.2f, 1f, 1f));

        public static BsColor Electric()
            => new BsColor(new Color(1f, 1f, .2f));

        public static BsColor Oil()
            => new BsColor(new Color(.3f, .3f, .4f));

        public static BsColor Water()
            => new BsColor(new Color(.4f, .4f, 1f));

        public static BsColor Honey()
            => new BsColor(new Color(.9f, .7f, 0f));

        public static BsColor Medicine()
            => new BsColor(new Color(.3f, 1f, .2f));

        public static BsColor Lava()
            => new BsColor(new Color(1f, .3f, .2f));

        public static BsColor Void()
            => new BsColor(new Color(0f, 0f, 0f));

        public static BsColor Ether()
            => new BsColor(new Color(1f, 1f, 1f));

        public override void Parse(string raw)
        {
            var parts = raw.Split(' ');
            tint = new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]),
                float.Parse(parts[3]));
            glow = parts[4][0] == '1';
        }

        public override string Stringify()
        {
            return $"{tint.r} {tint.g} {tint.b} {tint.a} {(glow ? '1' : '0')}";
        }
    }
}
