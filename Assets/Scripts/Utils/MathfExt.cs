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
    }
}
