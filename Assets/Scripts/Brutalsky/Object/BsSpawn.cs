using Brutalsky.Property;
using UnityEngine;

namespace Brutalsky.Object
{
    public class BsSpawn : BsObject
    {
        public int priority { get; set; }
        public int usages { get; private set; }

        public BsSpawn(BsTransform transform, int priority = 0)
        {
            this.transform = transform;
            this.priority = priority;
        }

        public BsSpawn()
        {
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

        public override void Parse(string[] raw)
        {
            transform = new BsTransform();
            transform.Parse(raw[0]);
            priority = int.Parse(raw[1]);
        }

        public override string[] Stringify()
        {
            return new[]
            {
                transform.Stringify(),
                priority.ToString()
            };
        }
    }
}
