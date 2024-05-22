using System;

namespace Brutalsky.Scripts.Utils;

public static class RandomExt
{
    public static bool NextBoolean(this Random _)
    {
        return _.NextInt64(2) == 1;
    }

    public static int NextInt32(this Random _, int maxValue)
    {
        return (int)_.NextInt64(maxValue);
    }

    public static int NextInt32(this Random _, int minValue, int maxValue)
    {
        return (int)_.NextInt64(minValue, maxValue);
    }

    public static float NextSingle(this Random _, float maxValue)
    {
        return _.NextSingle() * maxValue;
    }

    public static float NextSingle(this Random _, float minValue, float maxValue)
    {
        return _.NextSingle() * (maxValue - minValue) + minValue;
    }
}
