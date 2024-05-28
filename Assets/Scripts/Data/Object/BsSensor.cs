using Controllers;
using Controllers.Base;
using Data.Base;
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

        public BsSensor(string id = "", params string[] relatives) : base(id, relatives) { }

        protected override BsBehavior _Init(GameObject gameObject, BsObject[] relatedObjects)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<SensorController>();
            controller.Object = this;

            // Apply transform
            gameObject.transform.localPosition = Position;
            gameObject.transform.localScale = Size;

            return controller;
        }
    }
}
