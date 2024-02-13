using UnityEngine;

namespace Brutalsky
{
    public class BsSpawn
    {
        public Vector2 position { get; set; }
        public int priority { get; set; }
        public int usages { get; private set; }

        public Vector2 Use()
        {
            usages++;
            return position;
        }

        public void Reset()
        {
            usages = 0;
        }
    }
}
