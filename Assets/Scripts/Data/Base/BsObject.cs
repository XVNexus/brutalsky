using System;
using Controllers.Base;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

namespace Data.Base
{
    public abstract class BsObject : IHasId
    {
        public abstract GameObject Prefab { get; }
        public abstract string Tag { get; }
        public string Id { get; set; }
        public string[] Relatives { get; set; } = Array.Empty<string>();

        public Vector2 Position { get; set; } = Vector2.zero;
        public float Rotation { get; set; }
        public sbyte Layer { get; set; }

        [CanBeNull] public GameObject InstanceObject { get; private set; }
        [CanBeNull] public BsBehavior InstanceController { get; private set; }
        public bool Active { get; private set; }

        protected BsObject(string id, params string[] relatives)
        {
            Id = id;
            Relatives = relatives;
        }

        protected BsObject(string id)
        {
            Id = id;
        }

        protected abstract BsBehavior _Init(GameObject gameObject, BsObject[] relatedObjects);

        public void Activate(Transform parent, BsObject[] relatedObjects)
        {
            if (!parent) throw Errors.ParentObjectUnbuilt();
            InstanceObject = UnityEngine.Object.Instantiate(Prefab, parent);
            InstanceController = _Init(InstanceObject, relatedObjects);
            Active = true;
        }

        public void Deactivate()
        {
            UnityEngine.Object.Destroy(InstanceObject);
            InstanceObject = null;
            InstanceController = null;
            Active = false;
        }
    }
}
