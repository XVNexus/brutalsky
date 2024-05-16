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
        public override string Tag => Tags.SensorPrefix;
        public override bool HasLogic => true;

        public float Size { get; set; }
        public bool OnEnter { get; set; }
        public bool OnStay { get; set; }
        public bool OnExit { get; set; }

        public BsSensor(string id, ObjectTransform transform, bool simulated,
            float size, bool onEnter, bool onStay, bool onExit)
            : base(id, transform, ObjectLayer.Midground, simulated)
        {
            Size = size;
            OnEnter = onEnter;
            OnStay = onStay;
            OnExit = onExit;
        }

        public BsSensor()
        {
        }

        protected override BsBehavior _Init(GameObject gameObject, BsMap map)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<SensorController>();
            controller.Object = this;

            // Apply config
            if (!Simulated)
            {
                UnityEngine.Object.Destroy(gameObject.GetComponent<CircleCollider2D>());
            }

            // Apply size and position
            gameObject.transform.localScale = Vector2.one * Size;
            gameObject.transform.localPosition = Transform.Position;

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
                new(LcsType.Float, Size),
                new(LcsType.Bool, OnEnter),
                new(LcsType.Bool, OnStay),
                new(LcsType.Bool, OnExit)
            };
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            Size = (float)props[0].Value;
            OnEnter = (bool)props[1].Value;
            OnStay = (bool)props[2].Value;
            OnExit = (bool)props[3].Value;
        }
    }
}
