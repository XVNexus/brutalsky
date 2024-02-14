using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky
{
    public abstract class BsObject
    {
        public BsTransform transform { get; set; }
        [CanBeNull] public GameObject instanceObject { get; set; }
        [CanBeNull] public Component instanceComponent { get; set; }
        public bool active { get; set; }
    }
}
