using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using Utils.Constants;

namespace Controllers.Sensor
{
    public class SensorTriggerController : SubControllerBase<BsSensor>
    {
        // Controller metadata
        public override string Id => "trigger";
        public override bool IsUnused => !Master.Object.Simulated;

        // Local constants
        public const int MaxTriggeredFrames = 3;

        // Local variables
        public bool triggered;
        private int _triggeredFrames;

        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTrigger(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTrigger(other);
        }

        private void OnTrigger(Collider2D other)
        {
            // Update triggered status
            if (!other.CompareTag(Tags.PlayerGTag)) return;
            _triggeredFrames = MaxTriggeredFrames;
            triggered = true;
        }

        private void FixedUpdate()
        {
            // Update triggered status
            triggered = _triggeredFrames > 0;
            _triggeredFrames = Mathf.Max(_triggeredFrames - 1, 0);
        }
    }
}
