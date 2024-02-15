using UnityEngine;

namespace Brutalsky
{
    public class BsSpawn
    {
        public Vector2 position { get; set; }
        public int priority { get; set; }
        public int usages { get; private set; }

        public BsSpawn(Vector2 position, int priority = 0)
        {
            this.position = position;
            this.priority = priority;
        }

        public BsSpawn(float x, float y, int priority = 0)
        {
            position = new Vector2(x, y);
            this.priority = priority;
        }

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
