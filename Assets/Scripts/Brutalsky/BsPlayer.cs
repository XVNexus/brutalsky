using Controllers;
using Core;
using UnityEngine;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public string name { get; set; }
        public float health { get; set; }
        public Color color { get; set; }

        public BsPlayer(string name, float health, Color color)
        {
            this.name = name;
            this.health = health;
            this.color = color;
        }
    }
}
