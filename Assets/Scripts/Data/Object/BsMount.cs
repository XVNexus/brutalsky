using Controllers;
using Controllers.Base;
using Data.Base;
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

        public BsMount(string id = "", params string[] relatives) : base(id, relatives) { }

        protected override BsBehavior _Init(GameObject gameObject, BsObject[] relatedObjects)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<MountController>();
            controller.Object = this;

            // Apply transform
            gameObject.transform.localPosition = Position;

            return controller;
        }
    }
}
