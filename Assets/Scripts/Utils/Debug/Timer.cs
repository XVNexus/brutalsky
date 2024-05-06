using System;
using UnityEngine;

namespace Utils.Debug
{
    public class Timer
    {
        public string name { get; private set; }
        public long start { get; private set; }
        public long end { get; private set; }

        public Timer(string name)
        {
            this.name = name;
            start = DateTime.Now.Ticks;
        }

        public void Stop()
        {
            end = DateTime.Now.Ticks;
            var duration = TimeSpan.FromTicks(end - start);
            UnityEngine.Debug.Log($"Operation '{name}' took {duration.TotalMilliseconds} ms");
        }

        public static void Lag(int amount)
        {
            var x = 0f;
            for (var i = 0; i < amount * 1000000; i++)
            {
                x += Mathf.PI;
                x *= x;
            }
        }
    }
}
