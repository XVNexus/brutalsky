using Brutalsky.Base;
using Controllers;
using Core;
using UnityEngine;
using Utils;
using Utils.Constants;
using Utils.Object;

namespace Brutalsky
{
    public class BsPlayer : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.playerPrefab;
        public override string Tag => Tags.Player;

        public string Name { get; set; }
        public float Health { get; set; }
        public ObjectColor Color { get; set; }
        public bool Dummy { get; set; }

        public BsPlayer(string id, string name, float health, ObjectColor color, bool dummy = false) : base(id, new ObjectTransform())
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

        public void Refresh(BsMap map)
        {
            // Get reference to existing object and reset player
            InstanceObject.GetComponent<PlayerController>().Refresh();
        }

        protected override string[] _ToSrz()
        {
            return new[]
            {
                SrzUtils.Stringify(Name),
                SrzUtils.Stringify(Health),
                SrzUtils.Stringify(Color),
                SrzUtils.Stringify(Dummy)
            };
        }

        protected override void _FromSrz(string[] properties)
        {
            Name = SrzUtils.ParseString(properties[0]);
            Health = SrzUtils.ParseFloat(properties[1]);
            Color = SrzUtils.ParseColor(properties[2]);
            Dummy = SrzUtils.ParseBool(properties[3]);
        }
    }
}
