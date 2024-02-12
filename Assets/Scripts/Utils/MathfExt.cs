using UnityEngine;

namespace Utils
{
    public static class MathfExt
    {
        public static float TMP(float x, float threshold, float multiplier, float power)
        {
            return Mathf.Pow(Mathf.Max(x - threshold, 0) * multiplier, power);
        }

        public static float MoveTo(float from, float to, float step)
        {
            return from < to ? Mathf.Min(from + step, to)
                : from > to ? Mathf.Max(from - step, to)
                : from;
        }

        public static Vector2 Lerp2(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t));
        }

        public static Vector3 Lerp3(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
        }
    }
}
