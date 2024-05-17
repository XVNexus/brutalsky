using System;
using Brutalsky.Base;
using Brutalsky.Logic;
using Controllers;
using Controllers.Base;
using Controllers.Goal;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;

namespace Brutalsky.Object
{
    public class BsGoal : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pGoal;
        public override string Tag => Tags.GoalPrefix;

        public Vector2 Position { get; set; } = Vector2.zero;
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

        protected override object[] _ToLcs()
        {
            return new object[] { Position, Size, Color, Redirect };
        }

        protected override void _FromLcs(object[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++];
            Size = (Vector2)props[i++];
            Color = (Color)props[i++];
            Redirect = (uint)props[i++];
        }
    }
}
