using UnityEngine;

namespace Utils
{
    public static class Vector2Ext
    {
        public static Vector2 Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
        }
    }
}
