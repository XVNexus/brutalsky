using JetBrains.Annotations;
using UnityEngine;
using EventSystem = Core.EventSystem;

namespace Brutalsky
{
    public abstract class BsObject
    {
        public uint id { get; }

        public BsTransform transform { get; set; }
        [CanBeNull] public GameObject instanceObject { get; set; }
        [CanBeNull] public Component instanceComponent { get; set; }
        public bool active { get; set; }

        public BsObject()
        {
            id = EventSystem.random.NextUInt(uint.MaxValue);
        }
    }
}
