using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky
{
    public abstract class BsObject : MonoBehaviour
    {
        [CanBeNull] public GameObject instance;
        public bool active;

        protected abstract GameObject _Create();

        public bool Create()
        {
            if (active) return false;
            instance = _Create();
            active = true;
            return true;
        }

        public bool Destroy()
        {
            if (!active) return false;
            Destroy(instance);
            active = false;
            return true;
        }
    }
}