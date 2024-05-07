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
using Utils.Object;

namespace Brutalsky.Object
{
    public class BsMount : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pMount;
        public override string Tag => Tags.MountPrefix;
        public override bool HasLogic => true;

        public float ExitAngle { get; set; }

        public BsMount(string id, ObjectTransform transform, bool simulated, float exitAngle)
            : base(id, transform, ObjectLayer.Midground, simulated)
        {
            ExitAngle = exitAngle;
        }

        public BsMount()
        {
        }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<MountController>();
            controller.Object = this;

            // Apply position
            gameObject.transform.localPosition = Transform.Position;

            return controller;
        }

        protected override BsNode _RegisterLogic()
        {
            var grabController = ((MountController)InstanceController).GetSub<MountGrabController>("grab");
            return new BsNode(Array.Empty<float>(), new float[3], (_, _) =>
            {
                return new[] { BsMatrix.ToLogic(grabController.active),
                    grabController.input.x, grabController.input.y };
            });
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(LcsType.Float, ExitAngle)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            ExitAngle = (float)props[0].Value;
        }
    }
}
