using Controllers.Base;
using UnityEngine;

namespace Core
{
    public class TimeSystem : BsBehavior
    {
        // Static instance
        public static TimeSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        public int maxFps;

        // Local variables
        private bool _userPaused;
        private bool _forcePaused;
        private bool _forcePauseActive;

        // Init functions
        protected override void OnStart()
        {
            Application.targetFrameRate = maxFps;
        }

        // System functions
        public void Pause()
        {
            _userPaused = true;
            UpdateTimeScale();
        }

        public void Unpause()
        {
            _userPaused = false;
            UpdateTimeScale();
        }

        public void ForcePause()
        {
            _forcePauseActive = true;
            _forcePaused = true;
            UpdateTimeScale();
        }

        public void ForceUnpause()
        {
            _forcePauseActive = true;
            _forcePaused = false;
            UpdateTimeScale();
        }

        public void RemoveForcePause()
        {
            _forcePauseActive = false;
            UpdateTimeScale();
        }

        public void UpdateTimeScale()
        {
            Time.timeScale = (_forcePauseActive ? _forcePaused : _userPaused) ? 0f : 1f;
        }
    }
}
