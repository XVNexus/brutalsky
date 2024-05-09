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

        // Config options
        public int maxTriggeredFrames;

        // Exposed properties
        public bool Triggered { get; private set; }

        // Local variables
        private int _triggeredFrames;

        // Event functions
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Master.Object.OnEnter)
            {
                OnTrigger(other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (Master.Object.OnStay)
            {
                OnTrigger(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (Master.Object.OnExit)
            {
                OnTrigger(other);
            }
        }

        private void OnTrigger(Collider2D other)
        {
            // Update triggered status
            if (!other.CompareTag(Tags.PlayerTag)) return;
            _triggeredFrames = maxTriggeredFrames;
            Triggered = true;
        }

        private void FixedUpdate()
        {
            // Update triggered status
            Triggered = _triggeredFrames > 0;
            _triggeredFrames = Mathf.Max(_triggeredFrames - 1, 0);
        }
    }
}
