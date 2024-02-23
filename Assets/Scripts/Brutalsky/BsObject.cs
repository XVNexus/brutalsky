using Brutalsky.Object;
using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky
{
    public abstract class BsObject
    {
        public string id { get; set; }
        public BsTransform transform { get; set; }

        [CanBeNull] public GameObject instanceObject { get; set; }
        [CanBeNull] public Component instanceComponent { get; set; }
        public bool active { get; set; }

        protected BsObject(string id, BsTransform transform)
        {
            this.id = id;
            this.transform = transform;
        }
    }
}
