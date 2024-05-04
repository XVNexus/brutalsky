using UnityEngine;

namespace Utils.Ext
{
    public static class ColorExt
    {
        public static readonly Color Wood = new(.9f, .6f, .3f, 1f);
        public static readonly Color Metal = new(.9f, .9f, 1f, 1f);
        public static readonly Color Stone = new(.7f, .7f, .7f, 1f);
        public static readonly Color Ice = new(.5f, .8f, 1f, 1f);
        public static readonly Color Rubber = new(.7f, .4f, .9f, 1f);
        public static readonly Color Glue = new(.7f, .9f, .4f, 1f);
        public static readonly Color Medkit = new(.2f, 1f, 1f, 1f);
        public static readonly Color Electric = new(1f, 1f, .2f, 1f);
        public static readonly Color Oil = new(.3f, .3f, .4f, 1f);
        public static readonly Color Water = new(.4f, .4f, 1f, 1f);
        public static readonly Color Honey = new(.9f, .7f, 0f, 1f);
        public static readonly Color Medicine = new(.3f, 1f, .2f, 1f);
        public static readonly Color Lava = new(1f, .3f, .2f, 1f);
        public static readonly Color Void = new(0f, 0f, 0f, 1f);
        public static readonly Color Ether = new(1f, 1f, 1f, 1f);

        public static Color MultiplyTint(this Color _, float value)
        {
            return new Color(_.r * value, _.g * value, _.b * value, _.a);
        }

        public static Color SetAlpha(this Color _, float a)
        {
            return new Color(_.r, _.g, _.b, a);
        }

        public static Color MergeAlpha(this Color _)
        {
            return new Color(_.r * _.a, _.g * _.a, _.b * _.a);
        }

        public static Color StripAlpha(this Color _)
        {
            return new Color(_.r, _.g, _.b);
        }
    }
}
