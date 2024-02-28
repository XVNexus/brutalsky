using UnityEngine;

namespace Core
{
    public class TimeSystem : MonoBehaviour
    {
        public static TimeSystem current;
        private void Awake() => current = this;

        // Variables
        private bool paused;

        // Functions
        public bool Pause()
        {
            if (paused) return false;
            Time.timeScale = 0f;
            paused = true;
            return true;
        }

        public bool Unpause()
        {
            if (!paused) return false;
            Time.timeScale = 1f;
            paused = false;
            return true;
        }
    }
}
