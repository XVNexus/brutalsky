using UnityEngine;

namespace Brutalsky
{
    public class BsSpawn
    {
        public Vector2 position { get; set; }
        public int priority { get; set; }
        public int usages => _usages;
        private int _usages;

        public Vector2 Use()
        {
            _usages++;
            return position;
        }

        public void Reset()
        {
            _usages = 0;
        }
    }
}
