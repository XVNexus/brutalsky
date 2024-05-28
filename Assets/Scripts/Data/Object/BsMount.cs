using System;
using Controllers;
using Controllers.Base;
using Controllers.Mount;
using Data.Base;
using Data.Logic;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsMount : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pMount;
        public override string Tag => Tags.MountPrefix;

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
            return new BsNode(Tags.MountPrefix, Id)
            {
                Init = () => (Array.Empty<float>(), new float[3]),
                Update = _ => new[] { BsNode.ToLogic(grabController.Active),
                    grabController.Input.x, grabController.Input.y }
            };
        }
    }
}
