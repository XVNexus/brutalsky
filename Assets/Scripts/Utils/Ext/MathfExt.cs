using UnityEngine;

namespace Utils.Ext
{
    public static class MathfExt
    {
        // Common tools
        public static Rect Clamp(Rect rect, Rect bounds)
        {
            var min = Max(rect.min, bounds.min);
            var max = Min(rect.max, bounds.max);
            return new Rect(min, max - min);
        }

        public static Vector2 Clamp(Vector2 vector, Rect bounds)
        {
            return new Vector2
            (
                Mathf.Clamp(vector.x, bounds.xMin, bounds.xMax),
                Mathf.Clamp(vector.y, bounds.yMin, bounds.yMax)
            );
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }

        public static Vector2 Pow(Vector2 v, float p)
        {
            return v.normalized * Mathf.Pow(v.magnitude, p);
        }

        // Vector tools
        public static Vector2 ToVector(float angle, float magnitude)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
        }

        public static Vector2 ToVector(Vector2 polar)
        {
            return ToVector(polar.x, polar.y);
        }

        public static Vector2 ToPolar(float x, float y)
        {
            return ToPolar(new Vector2(x, y));
        }

        public static Vector2 ToPolar(Vector2 vector)
        {
            return new Vector2(Atan2(vector), vector.magnitude);
        }

        public static float Atan2(Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x);
        }

        public static Vector2 TranslateVector(Vector2 vector, Vector2 translation)
        {
            return vector + translation;
        }

        public static Vector2 RotateVector(Vector2 vector, float rotation)
        {
            var rad = rotation * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);
            return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }

        public static Vector2 TransformVector(Vector2 vector, Vector2 translation, float rotation)
        {
            return TranslateVector(RotateVector(vector, rotation), translation);
        }

        // Tween tools
        public static Vector2 MoveToLinear(Vector2 from, Vector2 to, float step)
        {
            return new Vector2(MoveToLinear(from.x, to.x, step), MoveToLinear(from.y, to.y, step));
        }

        public static Vector2 MoveToExponential(Vector2 from, Vector2 to, float factor)
        {
            return new Vector2(MoveToExponential(from.x, to.x, factor), MoveToExponential(from.y, to.y, factor));
        }

        public static float MoveToLinear(float from, float to, float step)
        {
            return from < to ? Mathf.Min(from + step, to) : from > to ? Mathf.Max(from - step, to) : from;
        }

        public static float MoveToExponential(float from, float to, float factor)
        {
            return from + (to - from) * factor;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t));
        }

        // Dataset tools
        public static float Mean(float a, float b)
        {
            return (a + b) * .5f;
        }

        public static Vector2 Mean(Vector2 a, Vector2 b)
        {
            return (a + b) * .5f;
        }

        public static float WeightedMean(float a, float aWeight, float b, float bWeight)
        {
            return (a * aWeight + b * bWeight) / (aWeight + bWeight);
        }

        public static Vector2 WeightedMean(Vector2 a, float aWeight, Vector2 b, float bWeight)
        {
            return (a * aWeight + b * bWeight) / (aWeight + bWeight);
        }

        // Advanced tools
        public static string ToBaseX(string charset, int numberBase10)
        {
            var result = "";

            var radix = charset.Length;
            var quotient = numberBase10;
            while (quotient != 0)
            {
                var remainder = quotient % radix;
                quotient /= radix;
                result = charset[remainder] + result;
            }

            return numberBase10 > 0 ? result : charset[0].ToString();
        }

        public static int ToBase10(string charset, string numberBaseX)
        {
            var result = 0;

            var radix = charset.Length;
            for (var i = 0; i < numberBaseX.Length; i++)
            {
                var power = (int)Mathf.Pow(radix, i);
                var digit = numberBaseX[numberBaseX.Length - 1 - i];
                result += charset.IndexOf(digit) * power;
            }

            return result;
        }
    }
}
