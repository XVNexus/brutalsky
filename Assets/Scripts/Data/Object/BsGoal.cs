using System;
using Controllers;
using Controllers.Base;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsGoal : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pGoal;
        public override string Tag => Tags.GoalPrefix;

        public override Func<GameObject, BsObject, BsObject[], BsBehavior> Init => (gob, obj, _) =>
        {
            // Link object to controller
            var goal = obj.As<BsGoal>();
            var controller = gob.GetComponent<GoalController>();
            controller.Object = goal;

            // Apply transform
            gob.transform.localPosition = goal.Position;
            gob.transform.localScale = goal.Size;

            return controller;
        };

        public Vector2 Size
        {
            get => Vector2Ext.FromLcs(Props[0]);
            set => Props[0] = value.ToLcs();
        }

        public Color Color
        {
            get => ColorExt.FromLcs(Props[1]);
            set => Props[1] = value.ToLcs();
        }

        public uint Redirect
        {
            get => (uint)Props[2];
            set => Props[2] = value;
        }

        public BsGoal(string id = "", params string[] relatives) : base(id, relatives)
        {
            Props = new[] { Vector2.one.ToLcs(), Color.white.ToLcs(), (uint)0 };
        }

        public BsGoal() { }
    }
}
