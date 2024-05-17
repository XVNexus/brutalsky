using System;
using Brutalsky.Base;
using Brutalsky.Logic;
using Controllers;
using Controllers.Base;
using Controllers.Mount;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;

namespace Brutalsky.Object
{
    public class BsMount : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pMount;
        public override string Tag => Tags.MountPrefix;

        public Vector2 Position { get; set; } = Vector2.zero;
        public Vector2 EjectionForce { get; set; } = Vector2.zero;

        public BsMount(string id = "") : base(id) { }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<MountController>();
            controller.Object = this;

            // Apply transform
            gameObject.transform.localPosition = Position;

            return controller;
        }

        protected override BsNode _RegisterLogic()
        {
            var grabController = ((MountController)InstanceController).GetSub<MountGrabController>("grab");
            return new BsNode(Array.Empty<float>(), new float[3], (_, _) =>
            {
                return new[] { BsMatrix.ToLogic(grabController.Active),
                    grabController.Input.x, grabController.Input.y };
            });
        }

        protected override object[] _ToLcs()
        {
            return new object[] { Position, EjectionForce };
        }

        protected override void _FromLcs(object[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++];
            EjectionForce = (Vector2)props[i++];
        }
    }
}
