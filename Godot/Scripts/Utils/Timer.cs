using System;
using Godot;

namespace Brutalsky.Scripts.Utils;

public class Timer
{
    public string Name { get; }
    public long Start { get; }

    public Timer(string name)
    {
        Name = name;
        Start = DateTime.Now.Ticks;
    }

    public void Stop()
    {
        var duration = TimeSpan.FromTicks(DateTime.Now.Ticks - Start);
        GD.Print($"Operation '{Name}' took {duration.TotalMilliseconds} ms");
    }

    public static void Lag(int amount)
    {
        var x = 0f;
        for (var i = 0; i < amount * 1000000; i++)
        {
            x += Mathf.Pi;
            x *= x;
        }
    }
}
