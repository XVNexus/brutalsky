using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class MathfExt
    {
        public static float InverseLerp(float a, float b, float value)
        {
            return (value - a) / (b - a);
        }

        public static float TMP(float x, float threshold, float multiplier = 1f, float power = 1f)
        {
            return Mathf.Pow(Mathf.Max(x - threshold, 0) * multiplier, power);
        }

        public static Vector2 RotateVector(Vector2 vector, float rotation)
        {
            /*var magnitude = vector.magnitude;
            var angle = Atan2(vector);
            return new Vector2(Mathf.Cos(angle + rotation), Mathf.Sin(angle + rotation)) * magnitude;*/
            var cos = Mathf.Cos(rotation);
            var sin = Mathf.Sin(rotation);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }

        public static float MoveTo(float from, float to, float step)
        {
            return from < to ? Mathf.Min(from + step, to)
                : from > to ? Mathf.Max(from - step, to)
                : from;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t));
        }

        public static float Mean(float a, float b)
        {
            return (a + b) / 2f;
        }

        public static Vector2 Mean(Vector2 a, Vector2 b)
        {
            return new Vector2(Mean(a.x, b.x), Mean(a.y, b.y));
        }

        public static float WeightedMean(float a, float aWeight, float b, float bWeight)
        {
            return (a * aWeight + b * bWeight) / (aWeight + bWeight);
        }

        public static Vector2 WeightedMean(Vector2 a, float aWeight, Vector2 b, float bWeight)
        {
            return new Vector2(WeightedMean(a.x, aWeight, b.x, bWeight),
                WeightedMean(a.y, aWeight, b.y, bWeight));
        }
    }
}
