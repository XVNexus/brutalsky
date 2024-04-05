using UnityEngine;
using Utils.Ext;

namespace Utils.Object
{
    public class ObjectColor
    {
        public Color Tint { get; set; }
        public float Alpha
        {
            get => Tint.a;
            set
            {
                var color = Tint;
                color.a = value;
                Tint = color;
            }
        }
        public bool Glow { get; set; }

        public ObjectColor(Color tint, bool glow = false)
        {
            Tint = tint;
            Glow = glow;
        }

        public ObjectColor(float r, float g, float b, float a = 1f, bool glow = false)
        {
            Tint = new Color(r, g, b, a);
            Glow = glow;
        }

        public static ObjectColor Wood(bool glow = false) => new(.9f, .6f, .3f, 1f, glow);

        public static ObjectColor Metal(bool glow = false) => new(.9f, .9f, 1f, 1f, glow);

        public static ObjectColor Stone(bool glow = false) => new(.7f, .7f, .7f, 1f, glow);

        public static ObjectColor Ice(bool glow = false) => new(.5f, .8f, 1f, 1f, glow);

        public static ObjectColor Rubber(bool glow = false) => new(.7f, .4f, .9f, 1f, glow);

        public static ObjectColor Glue(bool glow = false) => new(.7f, .9f, .4f, 1f, glow);

        public static ObjectColor Medkit(bool glow = true) => new(.2f, 1f, 1f, 1f, glow);

        public static ObjectColor Electric(bool glow = true) => new(1f, 1f, .2f, 1f, glow);

        public static ObjectColor Oil(bool glow = false) => new(.3f, .3f, .4f, 1f, glow);

        public static ObjectColor Water(bool glow = false) => new(.4f, .4f, 1f, 1f, glow);

        public static ObjectColor Honey(bool glow = false) => new(.9f, .7f, 0f, 1f, glow);

        public static ObjectColor Medicine(bool glow = true) => new(.3f, 1f, .2f, 1f, glow);

        public static ObjectColor Lava(bool glow = true) => new(1f, .3f, .2f, 1f, glow);

        public static ObjectColor Void(bool glow = false) => new(0f, 0f, 0f, 1f, glow);

        public static ObjectColor Ether(bool glow = false) => new(1f, 1f, 1f, 1f, glow);

        public static ObjectColor Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new ObjectColor(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]),
                float.Parse(parts[3]), BoolExt.Parse(parts[4]));
        }

        public override string ToString()
        {
            return $"{Tint.r} {Tint.g} {Tint.b} {Tint.a} {BoolExt.ToString(Glow)}";
        }
    }
}
