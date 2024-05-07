using UnityEngine;

namespace Utils.Ext
{
    public static class Vector2Ext
    {
        public static float Aspect(this Vector2 _)
        {
            return _.x / _.y;
        }
    }
}
