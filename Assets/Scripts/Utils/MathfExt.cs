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

        public static Vector2 Lerp2(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t));
        }

        public static Vector3 Lerp3(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
        }

        public static float Mean(float a, float b)
        {
            return (a + b) / 2f;
        }

        public static float WeightedMean(float valueA, float weightA, float valueB, float weightB)
        {
            return (valueA * weightA + valueB * weightB) / (weightA + weightB);
        }

        public static float Mean(params float[] values)
        {
            return values.Sum() / values.Length;
        }

        public static float WeightedMean(params float[] valuesAndWeights)
        {
            var valueSum = 0f;
            var weightSum = 0f;
            for (var i = 0; i < valuesAndWeights.Length - 1; i += 2)
            {
                var value = valuesAndWeights[i];
                var weight = valuesAndWeights[i + 1];
                valueSum += value * weight;
                weightSum += weight;
            }
            return valueSum / weightSum;
        }
    }
}
