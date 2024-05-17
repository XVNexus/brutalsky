using System.Collections.Generic;
using System.Linq;
using Brutalsky.Logic;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;

namespace Brutalsky.Base
{
    public abstract class BsObject
    {
        public abstract GameObject Prefab { get; }
        public abstract string Tag { get; }
        public string Id { get; set; }
        public string ParentTag { get; set; }
        public string ParentId { get; set; }
        public List<BsAddon> Addons { get; } = new();

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

        protected BsObject()
        {
        }

        protected abstract BsBehavior _Init(GameObject gameObject, BsMap map);

        [CanBeNull]
        protected virtual BsNode _RegisterLogic()
        {
            return null;
        }

        protected abstract LcsProp[] _ToLcs();

        protected abstract void _FromLcs(LcsProp[] props);

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
                : map.GetObject<BsObject>(ParentTag, ParentId).InstanceObject.transform;
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

        public LcsLine ToLcs()
        {
            var props = new List<LcsProp>
            {
                new(LcsType.String, Tag),
                new(LcsType.String, Id),
                new(LcsType.String, ParentTag),
                new(LcsType.String, ParentId)
            };
            props.AddRange(_ToLcs());
            return new LcsLine('#', props.ToArray(), Addons.Select(addon => addon.ToLcs()).ToList());
        }

        public static BsObject FromLcs(LcsLine line)
        {
            var result = ResourceSystem.GetTemplateObject((string)line.Props[0].Value);
            result.ParseLcs(line);
            return result;
        }

        private void ParseLcs(LcsLine line)
        {
            Id = (string)line.Props[1].Value;
            ParentTag = (string)line.Props[2].Value;
            ParentId = (string)line.Props[3].Value;
            _FromLcs(line.Props[4..]);
            foreach (var child in line.Children)
            {
                Addons.Add(BsAddon.FromLcs(child));
            }
        }

        public override string ToString()
        {
            return $"OBJECT :: {Tag}.{Id}";
        }
    }
}
