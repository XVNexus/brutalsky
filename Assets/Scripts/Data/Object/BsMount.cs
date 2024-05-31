using System;
using Controllers;
using Controllers.Base;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsMount : BsObject
    {
        public override GameObject Prefab => ResourceSystem._.pMount;
        public override string Tag => Tags.MountPrefix;
        public override Func<GameObject, BsObject, BsObject[], BsBehavior> Init => (gob, obj, _) =>
        {
            // Link object to controller
            var mount = obj.As<BsMount>();
            var controller = gob.GetComponent<MountController>();
            controller.Object = mount;

            // Apply transform
            gob.transform.localPosition = mount.Position;

            return controller;
        };

        public Vector2 EjectionForce
        {
            get => Vector2Ext.FromLcs(Props[0]);
            set => Props[0] = value.ToLcs();
        }

        public BsMount(string id = "", params string[] relatives) : base(id, relatives)
        {
            Props = new[] { Vector2.zero.ToLcs() };
        }

        public BsMount() { }
    }
}
