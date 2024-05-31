using Lcs;
using UnityEngine;

namespace Extensions
{
    public static class Vector2Ext
    {
        public static float Aspect(this Vector2 _)
        {
            return _.x / _.y;
        }

        public static Vector2 ForceAspect(this Vector2 _, float aspect, bool minifyOrMaxify = true)
        {
            return (minifyOrMaxify ? _.Aspect() > aspect : _.Aspect() < aspect)
                ? new Vector2(_.x, _.x / aspect)
                : new Vector2(_.y * aspect, _.y);
        }

        public static object ToLcs(this Vector2 _)
        {
            return new object[] { _.x, _.y };
        }

        public static Vector2 FromLcs(object prop)
        {
            var parts = (object[])prop;
            return new Vector2((float)parts[0], (float)parts[1]);
        }
    }
}
