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
        public override GameObject Prefab => PlayerSystem._.playerPrefab;
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

        public static float CalculateDamage(float impactForce)
        {
            return MathfExt.TMP(impactForce, 20f, .5f);
        }
    }
}
