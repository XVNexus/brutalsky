using JetBrains.Annotations;
using UnityEngine;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsAddon
    {
        public abstract string Tag { get; }
        public string Id { get; set; }
        public ObjectTransform Transform { get; set; }

        [CanBeNull] public Component InstanceComponent { get; set; }
        public bool Active { get; set; }

        protected BsAddon(string id, ObjectTransform transform)
        {
            Id = id;
            Transform = transform;
        }

        protected BsAddon()
        {
        }

        public abstract void Init(GameObject gameObject, BsObject parentObject, BsMap map);

        public bool Activate(Component instanceComponent)
        {
            if (Active) return false;
            InstanceComponent = instanceComponent;
            Active = true;
            return true;
        }

        public bool Deactivate()
        {
            if (!Active) return false;
            InstanceComponent = null;
            Active = false;
            return true;
        }
    }
}
