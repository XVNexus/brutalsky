using UnityEngine;

namespace Utils
{
    public static class MathfExt
    {
        public static float TMP(float x, float threshold, float multiplier, float power)
        {
            return Mathf.Pow(Mathf.Max(x - threshold, 0) * multiplier, power);
        }
    }
}
