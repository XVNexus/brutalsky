using Config;
using Controllers.Base;
using UnityEngine;

namespace Systems
{
    public class TimeSystem : BsBehavior
    {
        // Static instance
        public static TimeSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        private int _cfgMaxFps;

        // Local variables
        private bool _userPaused;
        private bool _forcePaused;
        private bool _forcePauseActive;

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnConfigUpdate += OnConfigUpdate;
        }

        private void OnDestroy()
        {
            EventSystem._.OnConfigUpdate -= OnConfigUpdate;
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

        // Event functions
        private void OnConfigUpdate(ConfigDelta cfg)
        {
            _cfgMaxFps = (int)cfg.GetOrDefault("tmsys", "mxfps", _cfgMaxFps);

            Application.targetFrameRate = _cfgMaxFps;
        }
    }
}
