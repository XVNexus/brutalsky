using UnityEngine;

namespace Utils.Object
{
    public class ObjectColor
    {
        public Color Color { get; set; }
        public Color Tint => new Color(Color.r * Color.a, Color.g * Color.a, Color.b * Color.a);
        public float Alpha
        {
            get => Color.a;
            set
            {
                var color = Color;
                color.a = value;
                Color = color;
            }
        }
        public bool Glow { get; set; }

        public ObjectColor(Color color, bool glow = false)
        {
            Color = color;
            Glow = glow;
        }

        public ObjectColor(float r, float g, float b, float a = 1f, bool glow = false)
        {
            Color = new Color(r, g, b, a);
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
    }
}
