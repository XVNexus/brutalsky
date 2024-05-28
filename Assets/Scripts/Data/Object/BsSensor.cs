using System;
using Controllers;
using Controllers.Base;
using Controllers.Sensor;
using Data.Base;
using Data.Logic;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsSensor : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pSensor;
        public override string Tag => Tags.SensorPrefix;

        public Vector2 Size { get; set; } = Vector2.one;
        public bool OnEnter { get; set; }
        public bool OnStay { get; set; } = true;
        public bool OnExit { get; set; }
        public (bool, bool, bool) TriggerMode
        {
            get => (OnEnter, OnStay, OnExit);
            set
            {
                OnEnter = value.Item1;
                OnStay = value.Item2;
                OnExit = value.Item3;
            }
        }

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
            return new BsNode(Tags.MountPrefix, Id)
            {
                Init = () => (Array.Empty<float>(), new float[1]),
                Update = _ => new[] { BsNode.ToLogic(triggerController.Triggered) }
            };
        }
    }
}
