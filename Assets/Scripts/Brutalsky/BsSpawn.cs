using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky
{
    public class BsSpawn : BsObject
    {
        public int Priority { get; set; }
        public int Usages { get; private set; }

        public BsSpawn(string id, BsTransform transform, int priority = 0) : base(id, transform)
        {
            Priority = priority;
        }

        public Vector2 Use()
        {
            Usages++;
            return Transform.Position;
        }

        public void Reset()
        {
            Usages = 0;
        }
    }
}
