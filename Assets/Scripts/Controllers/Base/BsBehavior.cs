using UnityEngine;

namespace Controllers.Base
{
    public abstract class BsBehavior : MonoBehaviour
    {
        public const byte Unloaded = 0;
        public const byte Started = 1;
        public const byte Loaded = 2;
        public const byte Ready = 3;

        public byte Status { get; private set; } = Unloaded;

        // Phase 1: Initialize all components and subscribe to event system events
        public void Start()
        {
            OnStart();
            Status = Started;
            Invoke(nameof(Load), 0f);
        }

        protected virtual void OnStart()
        {
        }

        // Phase 2: Delete any unused controller modules and initialize ui panels
        private void Load()
        {
            OnLoad();
            Status = Loaded;
            Invoke(nameof(Link), 0f);
        }

        protected virtual void OnLoad()
        {
        }

        // Phase 3: Resolve any controller module dependencies and establish communication channels between modules
        private void Link()
        {
            OnLink();
            Status = Ready;
        }

        protected virtual void OnLink()
        {
        }
    }
}
