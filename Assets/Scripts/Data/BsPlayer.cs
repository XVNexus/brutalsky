using Controllers;
using Controllers.Base;
using Data.Base;
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

        public byte Type { get; set; } = TypeDummy;
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
    }
}
