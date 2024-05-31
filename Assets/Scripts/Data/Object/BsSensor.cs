using Controllers;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsSensor : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pSensor;
        public override string Tag => Tags.SensorPrefix;

        public Vector2 Size
        {
            get => Vector2Ext.FromLcs(Props[0]);
            set => Props[0] = value.ToLcs();
        }

        public bool OnEnter
        {
            get => (bool)Props[1];
            set => Props[1] = value;
        }

        public bool OnStay
        {
            get => (bool)Props[2];
            set => Props[2] = value;
        }

        public bool OnExit
        {
            get => (bool)Props[3];
            set => Props[3] = value;
        }

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

        public BsSensor(string id = "", params string[] relatives) : base(id, relatives)
        {
            Props = new[] { Vector2.one.ToLcs(), false, true, false };
            Init = (gob, obj, _) =>
            {
                // Link object to controller
                var sensor = obj.As<BsSensor>();
                var controller = gob.GetComponent<SensorController>();
                controller.Object = sensor;

                // Apply transform
                gob.transform.localPosition = sensor.Position;
                gob.transform.localScale = sensor.Size;

                return controller;
            };
        }

        public BsSensor() { }
    }
}
