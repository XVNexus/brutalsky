using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;
using Utils.Player;

namespace Brutalsky.Object
{
    public class BsPlayer : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPlayer;
        public override string Tag => Tags.PlayerPrefix;
        public override bool HasLogic => false;

        public PlayerType Type { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public float Health { get; set; }

        public BsPlayer(PlayerType type, string name, Color color, float health = 100f)
            : base(name, new ObjectTransform(), ObjectLayer.Midground, true)
        {
            Type = type;
            Name = name;
            Color = color;
            Health = health;
        }

        public BsPlayer()
        {
        }

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
                new(LcsType.String, Name),
                new(LcsType.Color, Color),
                new(LcsType.Float, Health)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            Type = (PlayerType)props[0].Value;
            Name = (string)props[1].Value;
            Color = (Color)props[2].Value;
            Health = (float)props[3].Value;
        }
    }
}
