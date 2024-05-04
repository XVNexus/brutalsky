using Controllers.Base;
using UnityEngine;

namespace Core
{
    public class TimeSystem : BsBehavior
    {
        // Static instance
        public static TimeSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Local variables
        public int maxFps;

        // Init functions
        protected override void OnStart()
        {
            Application.targetFrameRate = maxFps;
        }

        // System functions
        public static void Pause()
        {
            Time.timeScale = 0f;
        }

        public static void Unpause()
        {
            Time.timeScale = 1f;
        }
    }
}
