using JetBrains.Annotations;
using UnityEngine;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsObject
    {
        public string Id { get; set; }
        public ObjectTransform Transform { get; set; }

        [CanBeNull] public GameObject InstanceObject { get; set; }
        [CanBeNull] public Component InstanceComponent { get; set; }
        public bool Active { get; set; }

        protected BsObject(string id, ObjectTransform transform)
        {
            Id = id;
            Transform = transform;
        }
    }
}
