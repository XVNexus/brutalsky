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
        private bool _paused;

        // System functions
        public bool Pause()
        {
            if (_paused) return false;
            Time.timeScale = 0f;
            _paused = true;
            return true;
        }

        public bool Unpause()
        {
            if (!_paused) return false;
            Time.timeScale = 1f;
            _paused = false;
            return true;
        }
    }
}
