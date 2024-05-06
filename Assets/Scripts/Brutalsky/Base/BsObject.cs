using System;
using System.Collections.Generic;
using System.Linq;
using Brutalsky.Logic;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsObject
    {
        public abstract GameObject Prefab { get; }
        public abstract string Tag { get; }
        public abstract bool HasLogic { get; }
        public string Id { get => _id; set => _id = MapSystem.CleanId(value); }
        private string _id;
        public string ParentTag { get; set; }
        public string ParentId { get => _parentId; set => _parentId = MapSystem.CleanId(value); }
        private string _parentId;
        public ObjectTransform Transform { get; set; }
        public ObjectLayer Layer { get; set; }
        public bool Simulated { get; set; }
        public List<BsAddon> Addons { get; } = new();

        [CanBeNull] public GameObject InstanceObject { get; private set; }
        [CanBeNull] public BsBehavior InstanceController { get; private set; }
        public bool Active { get; private set; }

        protected BsObject(string id, string parentTag, string parentId, ObjectTransform transform, ObjectLayer layer, bool simulated)
        {
            Id = id;
            ParentTag = parentTag;
            ParentId = parentId;
            Transform = transform;
            Layer = layer;
            Simulated = simulated;
        }

        protected BsObject(string id, ObjectTransform transform, ObjectLayer layer, bool simulated)
        {
            Id = id;
            ParentTag = "";
            ParentId = "";
            Transform = transform;
            Layer = layer;
            Simulated = simulated;
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

        public List<BsNode> RegisterLogic()
        {
            var result = new List<BsNode>();
            if (HasLogic)
            {
                var objectNode = _RegisterLogic();
                objectNode.Tag = Tag;
                result.Add(objectNode);
            }
            foreach (var addon in Addons.Where(addon => addon.HasLogic))
            {
                var addonNode = addon.RegisterLogic();
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
                new(LcsType.String, ParentId),
                new(LcsType.Transform, Transform),
                new(LcsType.Layer, Layer),
                new(LcsType.Bool, Simulated),
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
            Transform = (ObjectTransform)line.Props[4].Value;
            Layer = (ObjectLayer)line.Props[5].Value;
            Simulated = (bool)line.Props[6].Value;
            try
            {
                _FromLcs(line.Props[7..]);
            }
            catch (Exception ex)
            {
                throw Errors.ErrorWhile("parsing LCS line", line, ex);
            }
            foreach (var child in line.Children)
            {
                Addons.Add(BsAddon.FromLcs(child));
            }
        }
    }
}
