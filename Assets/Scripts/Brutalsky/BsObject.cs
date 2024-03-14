using Brutalsky.Object;
using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky
{
    public abstract class BsObject
    {
        public string Id { get; set; }
        public BsTransform Transform { get; set; }

        [CanBeNull] public GameObject InstanceObject { get; set; }
        [CanBeNull] public Component InstanceComponent { get; set; }
        public bool Active { get; set; }

        protected BsObject(string id, BsTransform transform)
        {
            Id = id;
            Transform = transform;
        }
    }
}
