using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky
{
    public class BsSpawn : BsObject
    {
        public int priority { get; set; }
        public int usages { get; private set; }

        public BsSpawn(string id, BsTransform transform, int priority = 0) : base(id)
        {
            this.transform = transform;
            this.priority = priority;
        }

        public Vector2 Use()
        {
            usages++;
            return transform.position;
        }

        public void Reset()
        {
            usages = 0;
        }
    }
}
