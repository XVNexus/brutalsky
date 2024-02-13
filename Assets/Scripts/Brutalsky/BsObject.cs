using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky
{
    public abstract class BsObject
    {
        public BsTransform transform { get; set; }
        [CanBeNull] public GameObject instance { get; set; }
        public bool active { get; set; }
    }
}
