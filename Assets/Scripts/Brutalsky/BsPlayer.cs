using Brutalsky.Base;
using Controllers;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pPlayer;
        public override char Tag => Tags.PlayerSym;

        public string Name { get; set; }
        public float Health { get; set; }
        public ObjectColor Color { get; set; }
        public bool Dummy { get; set; }

        public BsPlayer(string name, float health, ObjectColor color, bool dummy = false)
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

        protected override Component _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<PlayerController>();
            controller.Object = this;

            // Apply config
            controller.GetComponent<SpriteRenderer>().color = Color.Tint;

            return controller;
        }

        public void Refresh()
        {
            // Get reference to existing object and reset player
            InstanceObject.GetComponent<PlayerController>().Refresh();
        }

        protected override string[] _ToLcs()
        {
            return new[]
            {
                LcsParser.Stringify(Name),
                LcsParser.Stringify(Health),
                LcsParser.Stringify(Color),
                LcsParser.Stringify(Dummy)
            };
        }

        protected override void _FromLcs(string[] properties)
        {
            Name = LcsParser.ParseString(properties[0]);
            Health = LcsParser.ParseFloat(properties[1]);
            Color = LcsParser.ParseColor(properties[2]);
            Dummy = LcsParser.ParseBool(properties[3]);
        }
    }
}
