using UnityEngine;

namespace Utils.Ext
{
    public static class RectExt
    {
        public static float Aspect(this Rect _)
        {
            return _.size.Aspect();
        }

        public static Rect ForceAspect(this Rect _, float aspect, bool minifyOrMaxify = true)
        {
            var newSize = (minifyOrMaxify ? _.Aspect() > aspect : _.Aspect() < aspect)
                ? new Vector2(_.width, _.width / aspect)
                : new Vector2(_.height * aspect, _.height);
            return new Rect(
                new Vector2(_.x - (newSize.x - _.width) * .5f, _.y - (newSize.y - _.height) * .5f), newSize);
        }

        public static Rect Expand(this Rect _, float growth)
        {
            return new Rect(_.x - growth, _.y - growth, _.width + growth * 2f, _.height + growth * 2f);
        }
    }
}
