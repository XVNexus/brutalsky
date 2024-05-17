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

namespace Brutalsky.Object
{
    public class BsSensor : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pSensor;
        public override string Tag => Tags.SensorPrefix;

        public Vector2 Position { get; set; } = Vector2.zero;
        public Vector2 Size { get; set; } = Vector2.one;
        public bool OnEnter { get; set; }
        public bool OnStay { get; set; } = true;
        public bool OnExit { get; set; }

        public BsSensor(string id = "") : base(id) { }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<SensorController>();
            controller.Object = this;

            // Apply transform
            gameObject.transform.localPosition = Position;
            gameObject.transform.localScale = Size;

            return controller;
        }

        protected override BsNode _RegisterLogic()
        {
            var triggerController = ((SensorController)InstanceController).GetSub<SensorTriggerController>("trigger");
            return new BsNode(Array.Empty<float>(), new float[1], (_, _) =>
            {
                return new[] { BsMatrix.ToLogic(triggerController.Triggered) };
            });
        }

        protected override LcsProp[] _ToLcs()
        {
            return new LcsProp[]
            {
                new(Position),
                new(Size),
                new(OnEnter),
                new(OnStay),
                new(OnExit)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            var i = 0;
            Position = (Vector2)props[i++].Value;
            Size = (Vector2)props[i++].Value;
            OnEnter = (bool)props[i++].Value;
            OnStay = (bool)props[i++].Value;
            OnExit = (bool)props[i++].Value;
        }
    }
}
