using UnityEngine;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public string name { get; set; }
        public float health { get; set; }
        public Color color { get; set; }
        public bool dummy { get; set; }

        public BsPlayer(string name, float health, Color color, bool dummy = false)
        {
            this.name = name;
            this.health = health;
            this.color = color;
            this.dummy = dummy;
        }
    }
}
