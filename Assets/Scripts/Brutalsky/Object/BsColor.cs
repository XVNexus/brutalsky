using UnityEngine;
using Utils;

namespace Brutalsky.Object
{
    public class BsColor
    {
        public Color tint { get; set; }
        public float alpha
        {
            get => tint.a;
            set
            {
                var color = tint;
                color.a = value;
                tint = color;
            }
        }
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

        public static BsColor Wood(bool glow = false)
            => new BsColor(.9f, .6f, .3f, 1f, glow);

        public static BsColor Metal(bool glow = false)
            => new BsColor(.9f, .9f, 1f, 1f, glow);

        public static BsColor Stone(bool glow = false)
            => new BsColor(.7f, .7f, .7f, 1f, glow);

        public static BsColor Ice(bool glow = false)
            => new BsColor(.5f, .8f, 1f, 1f, glow);

        public static BsColor Rubber(bool glow = false)
            => new BsColor(.7f, .4f, .9f, 1f, glow);

        public static BsColor Glue(bool glow = false)
            => new BsColor(.7f, .9f, .4f, 1f, glow);

        public static BsColor Medkit(bool glow = true)
            => new BsColor(.2f, 1f, 1f, 1f, glow);

        public static BsColor Electric(bool glow = true)
            => new BsColor(1f, 1f, .2f, 1f, glow);

        public static BsColor Oil(bool glow = false)
            => new BsColor(.3f, .3f, .4f, 1f, glow);

        public static BsColor Water(bool glow = false)
            => new BsColor(.4f, .4f, 1f, 1f, glow);

        public static BsColor Honey(bool glow = false)
            => new BsColor(.9f, .7f, 0f, 1f, glow);

        public static BsColor Medicine(bool glow = true)
            => new BsColor(.3f, 1f, .2f, 1f, glow);

        public static BsColor Lava(bool glow = true)
            => new BsColor(1f, .3f, .2f, 1f, glow);

        public static BsColor Void(bool glow = false)
            => new BsColor(0f, 0f, 0f, 1f, glow);

        public static BsColor Ether(bool glow = false)
            => new BsColor(1f, 1f, 1f, 1f, glow);

        public static BsColor Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new BsColor(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]),
                float.Parse(parts[3]), BoolExt.Parse(parts[4]));
        }

        public override string ToString()
        {
            return $"{tint.r} {tint.g} {tint.b} {tint.a} {BoolExt.ToString(glow)}";
        }
    }
}
