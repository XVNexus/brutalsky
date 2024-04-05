using System.Collections.Generic;
using Brutalsky.Base;
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
            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, string> _ToSrz()
        {
            throw new System.NotImplementedException();
        }

        protected override void _FromSrz(Dictionary<string, string> properties)
        {
            throw new System.NotImplementedException();
        }
    }
}
