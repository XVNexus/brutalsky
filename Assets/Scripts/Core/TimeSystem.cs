using UnityEngine;

namespace Core
{
    public class TimeSystem : BsBehavior
    {
        public static TimeSystem _ { get; private set; }
        private void Awake() => _ = this;

        private bool _paused;

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
