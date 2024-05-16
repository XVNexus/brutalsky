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
        public float Size { get; set; } = 1f;
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
            gameObject.transform.localScale = Vector2.one * Size;

            return controller;
        }

        protected override BsNode _RegisterLogic()
        {
            var redirectController = ((GoalController)InstanceController).GetSub<GoalRedirectController>("redirect");
            return new BsNode(new float[1], Array.Empty<float>(), (inputs, _) =>
            {
                if (BsMatrix.ToBool(inputs[0]))
                {
                    redirectController.ActivateRedirect();
                }
                return Array.Empty<float>();
            });
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(LcsType.Float2, Position),
                new(LcsType.Float, Size),
                new(LcsType.Color, Color),
                new(LcsType.UInt, Redirect)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++].Value;
            Size = (float)props[i++].Value;
            Color = (Color)props[i++].Value;
            Redirect = (uint)props[i++].Value;
        }
    }
}
