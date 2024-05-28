using Controllers;
using Controllers.Base;
using Data.Base;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsGoal : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pGoal;
        public override string Tag => Tags.GoalPrefix;

        public Vector2 Size { get; set; } = Vector2.one;
        public Color Color { get; set; } = Color.white;
        public uint Redirect { get; set; }

        public BsGoal(string id = "") : base(id) { }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<GoalController>();
            controller.Object = this;

            // Apply transform
            gameObject.transform.localPosition = Position;
            gameObject.transform.localScale = Size;

            return controller;
        }
    }
}
