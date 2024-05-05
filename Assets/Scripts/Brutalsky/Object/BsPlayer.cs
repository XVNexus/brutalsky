using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Object
{
    public class BsPlayer : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPlayer;
        public override string Tag => Tags.PlayerLTag;
        public override bool HasLogic => false;

        public string Name { get; set; }
        public float Health { get; set; }
        public Color Color { get; set; }
        public bool Dummy { get; set; }

        public BsPlayer(string name, float health, Color color, bool dummy = false)
            : base(name, new ObjectTransform(), ObjectLayer.Midground, true)
        {
            Name = name;
            Health = health;
            Color = color;
            Dummy = dummy;
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

        protected override string[] _ToLcs()
        {
            return new[]
            {
                Stringifier.GetString(Name),
                Stringifier.GetString(Health),
                Stringifier.GetString(Color),
                Stringifier.GetString(Dummy)
            };
        }

        protected override void _FromLcs(string[] properties)
        {
            Name = Stringifier.ToString(properties[0]);
            Health = Stringifier.ToSingle(properties[1]);
            Color = Stringifier.ToColor(properties[2]);
            Dummy = Stringifier.ToBoolean(properties[3]);
        }
    }
}
