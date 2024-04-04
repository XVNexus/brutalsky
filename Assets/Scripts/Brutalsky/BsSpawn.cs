using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky
{
    public class BsSpawn
    {
        public Vector2 Position { get; set; }
        public int Priority { get; set; }
        public int Usages { get; private set; }

        public BsSpawn(Vector2 position, int priority = 0)
        {
            Position = position;
            Priority = priority;
        }

        public Vector2 Use()
        {
            Usages++;
            return Position;
        }

        public void Reset()
        {
            Usages = 0;
        }
    }
}
