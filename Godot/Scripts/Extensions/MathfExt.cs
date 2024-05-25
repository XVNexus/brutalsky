using System;
using Godot;

namespace Brutalsky.Scripts.Extensions;

public static class MathfExt
{
    public const float EqualityThreshold = 1e-3f;

    // Common tools
    public static bool Approximately(float a, float b)
    {
        return Math.Abs(a - b) < EqualityThreshold;
    }

    public static Rect2 Clamp(Rect2 rect, Rect2 bounds)
    {
        var min = Max(rect.Position, bounds.Position);
        var max = Min(rect.Corner(), bounds.Corner());
        return new Rect2(min, max - min);
    }

    public static Vector2 Clamp(Vector2 vector, Rect2 bounds)
    {
        return new Vector2
        (
            Mathf.Clamp(vector.X, bounds.Position.X, bounds.Corner().X),
            Mathf.Clamp(vector.Y, bounds.Position.Y, bounds.Corner().Y)
        );
    }

    public static Vector2 Min(Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.Min(a.X, b.X), Mathf.Min(a.Y, b.Y));
    }

    public static Vector2 Max(Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));
    }

    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
    {
        return Min(Max(value, min), max);
    }

    public static Vector2 Pow(Vector2 v, float p)
    {
        return v.Normalized() * Mathf.Pow(v.Length(), p);
    }

    public static Vector2 Round(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.X), Mathf.Round(v.Y));
    }

    public static float PowSign(float f, float p)
    {
        return Mathf.Abs(Mathf.Pow(f, p)) * Mathf.Sin(f);
    }

    public static float BellPoint(float t, float center, float variance, float power = 2f)
    {
        return PowSign(t, power) * variance + center;
    }

    // Vector tools
    public static Vector2 ToVector(float angle, float magnitude)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
    }

    public static Vector2 ToVector(Vector2 polar)
    {
        return ToVector(polar.X, polar.Y);
    }

    public static Vector2 ToPolar(float x, float y)
    {
        return ToPolar(new Vector2(x, y));
    }

    public static Vector2 ToPolar(Vector2 vector)
    {
        return new Vector2(Atan2(vector), vector.Length());
    }

    public static float Atan2(Vector2 vector)
    {
        return Mathf.Atan2(vector.Y, vector.X);
    }

    public static Vector2 TranslateVector(Vector2 vector, Vector2 translation)
    {
        return vector + translation;
    }

    public static Vector2 RotateVector(Vector2 vector, float rotation)
    {
        var rad = Mathf.DegToRad(rotation);
        var cos = Mathf.Cos(rad);
        var sin = Mathf.Sin(rad);
        return new Vector2(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
    }

    public static Vector2 TransformVector(Vector2 vector, Vector2 translation, float rotation)
    {
        return TranslateVector(RotateVector(vector, rotation), translation);
    }

    // Tween tools
    public static Vector2 MoveToLinear(Vector2 from, Vector2 to, float step)
    {
        return new Vector2(MoveToLinear(from.X, to.X, step), MoveToLinear(from.Y, to.Y, step));
    }

    public static Vector2 MoveToExponential(Vector2 from, Vector2 to, float factor)
    {
        return new Vector2(MoveToExponential(from.X, to.X, factor), MoveToExponential(from.Y, to.Y, factor));
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
        return new Vector2(Mathf.Lerp(a.X, b.X, t), Mathf.Lerp(a.Y, b.Y, t));
    }

    // Dataset tools
    public static float Relerp(float t, float a, float b, float c, float d)
    {
        return Mathf.Lerp(c, d, Mathf.InverseLerp(a, b, t));
    }

    public static float Relerp(float t, Vector2 from, Vector2 to)
    {
        return Relerp(t, from.X, from.Y, to.X, to.Y);
    }

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
