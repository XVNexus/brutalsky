using Controllers;
using Core;
using UnityEngine;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public Color color { get; set; }
        public float health { get; set; }

        public BsPlayer(string id, Color color, float health) : base(id)
        {
            this.color = color;
            this.health = health;
        }

        protected override GameObject _Create()
        {
            var result = Instantiate(PrefabSystem.current.player);
            result.GetComponent<PlayerColorController>().playerColor = color;
            result.GetComponent<PlayerHealthController>().maxHealth = health;
            return result;
        }
    }
}
