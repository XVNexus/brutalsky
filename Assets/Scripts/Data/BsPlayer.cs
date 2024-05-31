using System;
using Controllers;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data
{
    public class BsPlayer : BsObject
    {
        public const byte TypeDummy = 0;
        public const byte TypeLocal1 = 1;
        public const byte TypeLocal2 = 2;
        public const byte TypeLocal3 = 3;
        public const byte TypeLocal4 = 4;
        public const byte TypeMain = 5;
        public const byte TypeBot = 6;

        public override GameObject Prefab => ResourceSystem._.pPlayer;
        public override string Tag => Tags.PlayerPrefix;

        public byte Type { get => (byte)Props[0]; set => Props[0] = value; }
        public Color Color { get => ColorExt.FromLcs(Props[1]); set => Props[1] = value.ToLcs(); }
        public float Health { get => (float)Props[2]; set => Props[2] = value; }

        public BsPlayer(string id = "") : base(id, Array.Empty<string>())
        {
            Props = new[] { TypeDummy, Color.white.ToLcs(), 100f };
            Init = (gob, obj, _) =>
            {
                // Link object to controller
                var player = obj.As<BsPlayer>();
                var controller = gob.GetComponent<PlayerController>();
                controller.Object = player;

                // Apply config
                controller.GetComponent<SpriteRenderer>().color = player.Color;

                return controller;
            };
        }

        public BsPlayer() { }
    }
}
