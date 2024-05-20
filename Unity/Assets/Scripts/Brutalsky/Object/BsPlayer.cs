using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Player;

namespace Brutalsky.Object
{
    public class BsPlayer : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPlayer;
        public override string Tag => Tags.PlayerPrefix;

        public PlayerType Type { get; set; } = PlayerType.Dummy;
        public Color Color { get; set; } = Color.white;
        public float Health { get; set; } = 100f;

        public BsPlayer(string id = "") : base(id) { }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<PlayerController>();
            controller.Object = this;

            // Apply config
            controller.GetComponent<SpriteRenderer>().color = Color;

            return controller;
        }

        protected override object[] _ToLcs()
        {
            return new object[] { Type, Color, Health };
        }

        protected override void _FromLcs(object[] props)
        {
            var i = 0;
            Type = (PlayerType)props[i++];
            Color = (Color)props[i++];
            Health = (float)props[i++];
        }
    }
}
