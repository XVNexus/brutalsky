using System;
using Controllers.Base;
using UnityEngine;

namespace Core
{
    public class TimeSystem : BsBehavior
    {
        // Static instance
        public static TimeSystem _ { get; private set; }
        private void Awake() => _ = this;

        // System functions
        public void Pause()
        {
            Time.timeScale = 0f;
        }

        public void Unpause()
        {
            Time.timeScale = 1f;
        }

        public void Slowdown()
        {
            throw new NotImplementedException();
        }

        public void Speedup()
        {
            throw new NotImplementedException();
        }
    }
}
