using UnityEngine;

namespace Brutalsky
{
    public class BsSpawn : BsObject
    {
        public int priority { get; set; }
        public int usages { get; private set; }

        public override char saveSymbol => 'N';

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

        public override void Parse(string[][] raw)
        {
            transform = new BsTransform(float.Parse(raw[0][0]), float.Parse(raw[0][1]), float.Parse(raw[0][2]));
            priority = int.Parse(raw[1][0]);
        }

        public override string[][] Stringify()
        {
            return new[]
            {
                new[] { transform.position.x.ToString(), transform.position.y.ToString(), transform.rotation.ToString() },
                new[] { priority.ToString() }
            };
        }
    }
}
