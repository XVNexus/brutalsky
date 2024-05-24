using System;
using System.Linq;

namespace Brutalsky.Scripts.Utils;

public static class Tags
{
    public const string Player = "Player";
    public const string Shape = "Shape";
    public const string Joint = "Joint";
    public const string Pool = "Pool";
    public const string Sensor = "Sensor";
    public const string Mount = "Mount";
    public const string Goal = "Goal";
    public const string Decal = "Decal";

    public static uint GenerateId(params string[] text)
    {
        return BitConverter.ToUInt32(BitConverter.GetBytes(
            text.Aggregate("", (current, item) => current + item).GetHashCode()), 0);
    }
}
