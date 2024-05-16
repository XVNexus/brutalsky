using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
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

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(LcsType.PlayerType, Type),
                new(LcsType.Color, Color),
                new(LcsType.Float, Health)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            var i = 0;
            Type = (PlayerType)props[i++].Value;
            Color = (Color)props[i++].Value;
            Health = (float)props[i++].Value;
        }
    }
}
