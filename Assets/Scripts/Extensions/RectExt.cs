using UnityEngine;

namespace Extensions
{
    public static class RectExt
    {
        public static float Aspect(this Rect _)
        {
            return _.size.Aspect();
        }

        public static Rect ForceAspect(this Rect _, float aspect, bool minifyOrMaxify = true)
        {
            var newSize = _.size.ForceAspect(aspect, minifyOrMaxify);
            return new Rect(
                new Vector2(_.x - (newSize.x - _.width) * .5f, _.y - (newSize.y - _.height) * .5f), newSize);
        }

        public static Rect Resize(this Rect _, Vector2 newSize)
        {
            return new Rect(_.position - (newSize - _.size) * .5f, newSize);
        }

        public static Rect Expand(this Rect _, float growth)
        {
            return new Rect(_.x - growth, _.y - growth, _.width + growth * 2f, _.height + growth * 2f);
        }

        public static object ToLcs(this Rect _)
        {
            return new object[] { _.x, _.y, _.width, _.height };
        }

        public static Rect FromLcs(object prop)
        {
            var parts = (object[])prop;
            return new Rect((float)parts[0], (float)parts[1], (float)parts[2], (float)parts[3]);
        }
    }
}
