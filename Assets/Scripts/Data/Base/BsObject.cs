using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Base;
using Data.Logic;
using JetBrains.Annotations;
using Lcs;
using Systems;
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
        public IdList<BsAddon> Addons { get; } = new();

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

        [CanBeNull]
        protected virtual BsNode _RegisterLogic()
        {
            return null;
        }

        public BsObject AppendAddon(BsAddon addon)
        {
            Addons.Add(addon);
            return this;
        }

        public List<BsNode> RegisterLogic()
        {
            var result = new List<BsNode>();
            var objectNode = _RegisterLogic();
            if (objectNode != null)
            {
                objectNode.Tag = Tag;
                result.Add(objectNode);
            }
            foreach (var addon in Addons.Values)
            {
                var addonNode = addon.RegisterLogic();
                if (addonNode == null) continue;
                addonNode.Tag = addon.Tag;
                result.Add(addonNode);
            }
            return result;
        }

        public void Activate(Transform parent, BsObject[] relatedObjects,
            Dictionary<string, BsObject[]> addonRelatedObjects)
        {
            if (!parent) throw Errors.ParentObjectUnbuilt();
            InstanceObject = UnityEngine.Object.Instantiate(Prefab, parent);
            InstanceController = _Init(InstanceObject, relatedObjects);
            Active = true;
            foreach (var addon in Addons.Values)
            {
                addon.Activate(InstanceObject, this, addonRelatedObjects[addon.Id]);
            }
        }

        public void Deactivate()
        {
            UnityEngine.Object.Destroy(InstanceObject);
            foreach (var addon in Addons.Values)
            {
                addon.Deactivate();
            }
            InstanceObject = null;
            InstanceController = null;
            Active = false;
        }
    }
}
