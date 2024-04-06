using System.Collections.Generic;
using Brutalsky.Base;
using Controllers;
using Controllers.Player;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
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

        protected override Dictionary<string, string> _ToSrz()
        {
            return new Dictionary<string, string>
            {
                ["nm"] = Name,
                ["ht"] = Health.ToString(),
                ["cl"] = Color.ToString(),
                ["dm"] = BoolExt.Stringify(Dummy)
            };
        }

        protected override void _FromSrz(Dictionary<string, string> properties)
        {
            Name = properties["nm"];
            Health = float.Parse(properties["ht"]);
            Color = ObjectColor.Parse(properties["cl"]);
            Dummy = BoolExt.Parse(properties["dm"]);
        }
    }
}
