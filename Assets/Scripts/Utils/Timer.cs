using System;
using UnityEngine;

namespace Utils
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
            Debug.Log($"Operation '{name}' took {duration.Milliseconds} ms");
        }
    }
}
