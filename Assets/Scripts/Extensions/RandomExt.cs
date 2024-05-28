using Unity.Mathematics;

namespace Extensions
{
    public static class RandomExt
    {
        public static float NextFloat(this Random _, float center, float variance, float curvature)
        {
            return MathfExt.PowSign(_.NextFloat(-1f, 1f), curvature) * variance + center;
        }
    }
}
