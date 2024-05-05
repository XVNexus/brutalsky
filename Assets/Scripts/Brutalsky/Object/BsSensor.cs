using System;
using Brutalsky.Base;
using Brutalsky.Logic;
using Controllers;
using Controllers.Base;
using Controllers.Sensor;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Object
{
    public class BsSensor : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pSensor;
        public override string Tag => Tags.SensorLTag;
        public override bool HasLogic => true;

        public Vector2 Size { get; set; }

        public BsSensor(string id, ObjectTransform transform, bool simulated, Vector2 size)
            : base(id, transform, ObjectLayer.Midground, simulated)
        {
            Size = size;
        }

        public BsSensor()
        {
        }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<SensorController>();
            controller.Object = this;

            // Apply size
            gameObject.transform.localScale = Size;

            // Apply position and rotation
            gameObject.transform.localPosition = Transform.Position;
            gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, Transform.Rotation);

            return controller;
        }

        protected override BsNode _RegisterLogic()
        {
            var triggerController = InstanceObject.GetComponent<SensorController>().GetSub<SensorTriggerController>("trigger");
            return new BsNode(Array.Empty<float>(), new float[1], (_, _) =>
            {
                return new[] { BsMatrix.ToLogic(triggerController.triggered) };
            });
        }

        protected override string[] _ToLcs()
        {
            return new[]
            {
                LcsParser.Stringify(Transform),
                LcsParser.Stringify(Size),
                LcsParser.Stringify(Simulated)
            };
        }

        protected override void _FromLcs(string[] properties)
        {
            Transform = LcsParser.ParseTransform(properties[0]);
            Size = LcsParser.ParseVector2(properties[1]);
            Simulated = LcsParser.ParseBool(properties[2]);
        }
    }
}
