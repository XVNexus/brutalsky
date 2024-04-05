using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsObject
    {
        public List<BsAddon> Addons { get; } = new();
        public abstract GameObject Prefab { get; }
        public abstract string Tag { get; }
        public string Id { get; set; }
        public ObjectTransform Transform { get; set; }

        [CanBeNull] public GameObject InstanceObject { get; set; }
        [CanBeNull] public Component InstanceController { get; set; }
        public bool Active { get; set; }

        protected BsObject(string id, ObjectTransform transform)
        {
            Id = id;
            Transform = transform;
        }

        protected BsObject()
        {
        }

        public void Init(GameObject gameObject, BsMap map)
        {
            _Init(gameObject);
            foreach (var addon in Addons)
            {
                addon.Init(gameObject, this, map);
            }
        }

        protected abstract void _Init(GameObject gameObject);

        public bool Activate(GameObject instanceObject, Component instanceController)
        {
            if (Active) return false;
            InstanceObject = instanceObject;
            InstanceController = instanceController;
            Active = true;
            return true;
        }

        public bool Deactivate()
        {
            if (!Active) return false;
            InstanceObject = null;
            InstanceController = null;
            Active = false;
            return true;
        }
    }
}
