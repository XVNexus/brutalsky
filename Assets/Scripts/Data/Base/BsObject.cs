using System.Collections.Generic;
using System.Linq;
using Controllers.Base;
using Data.Logic;
using JetBrains.Annotations;
using Lcs;
using Systems;
using UnityEngine;
using Utils;
using Utils.Constants;

namespace Data.Base
{
    public abstract class BsObject : IHasId
    {
        public abstract GameObject Prefab { get; }
        public abstract string Tag { get; }
        public string Id { get; set; }
        public string ParentTag { get; set; }
        public string ParentId { get; set; }
        public List<BsAddon> Addons { get; } = new();

        public Vector2 Position { get; set; } = Vector2.zero;
        public float Rotation { get; set; }
        public sbyte Layer { get; set; }

        [CanBeNull] public GameObject InstanceObject { get; private set; }
        [CanBeNull] public BsBehavior InstanceController { get; private set; }
        public bool Active { get; private set; }

        protected BsObject(string id, string parentTag, string parentId)
        {
            Id = id;
            ParentTag = parentTag;
            ParentId = parentId;
        }

        protected BsObject(string id)
        {
            Id = id;
            ParentTag = "";
            ParentId = "";
        }

        protected abstract BsBehavior _Init(GameObject gameObject, BsMap map);

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
            foreach (var addon in Addons)
            {
                var addonNode = addon.RegisterLogic();
                if (addonNode == null) continue;
                addonNode.Tag = addon.Tag;
                result.Add(addonNode);
            }
            return result;
        }

        public void Activate(Transform rootGameObject, BsMap map)
        {
            var parent = ParentId.Length == 0 ? rootGameObject
                : map.GetObject<BsObject>(ParentId).InstanceObject.transform;
            if (!parent) throw Errors.ParentObjectUnbuilt();
            InstanceObject = UnityEngine.Object.Instantiate(Prefab, parent);
            InstanceController = _Init(InstanceObject, map);
            Active = true;
            foreach (var addon in Addons)
            {
                addon.Activate(InstanceObject, this, map);
            }
        }

        public void Deactivate()
        {
            UnityEngine.Object.Destroy(InstanceObject);
            foreach (var addon in Addons)
            {
                addon.Deactivate();
            }
            InstanceObject = null;
            InstanceController = null;
            Active = false;
        }
    }
}
