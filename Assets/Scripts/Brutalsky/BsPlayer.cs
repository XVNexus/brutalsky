using Controllers;
using Core;
using UnityEngine;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public Color color { get; set; }
        public float health { get; set; }

        protected override GameObject _Create()
        {
            var result = Instantiate(PrefabSystem.current.player);
            result.GetComponent<PlayerColorController>().playerColor = color;
            result.GetComponent<PlayerHealthController>().maxHealth = health;
            return result;
        }
    }
}
