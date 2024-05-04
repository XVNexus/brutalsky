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
        public ObjectTransform Transform { get; set; }
        public ObjectLayer Layer { get; set; }
        public bool Simulated { get; set; }
        public List<BsAddon> Addons { get; } = new();

        [CanBeNull] public GameObject InstanceObject { get; private set; }
        [CanBeNull] public BsBehavior InstanceController { get; private set; }
        public bool Active { get; private set; }

        protected BsObject(string id, ObjectTransform transform, ObjectLayer layer, bool simulated)
        {
            Id = id;
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

        protected abstract string[] _ToLcs();

        protected abstract void _FromLcs(string[] properties);

        public List<BsNode> RegisterLogic()
        {
            var result = new List<BsNode>();
            var objNode = _RegisterLogic();
            if (objNode != null)
            {
                objNode.Tag = Tag;
                result.Add(objNode);
            }
            foreach (var addon in Addons)
            {
                var addonNode = addon.RegisterLogic();
                if (addonNode != null)
                {
                    addonNode.Tag = addon.Tag;
                    result.Add(addonNode);
                }
            }
            return result;
        }

        public void Activate(Transform parent, BsMap map)
        {
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
            return new LcsLine
            (
                '#',
                new[] { LcsParser.Stringify(Tag), LcsParser.Stringify(Id) },
                _ToLcs(),
                Addons.Select(addon => addon.ToLcs()).ToList()
            );
        }

        public static BsObject FromLcs(LcsLine line)
        {
            var result = ResourceSystem.GetTemplateObject(LcsParser.ParseString(line.Header[0]));
            result.ParseLcs(line);
            return result;
        }

        private void ParseLcs(LcsLine line)
        {
            Id = LcsParser.ParseString(line.Header[1]);
            try
            {
                _FromLcs(line.Properties);
            }
            catch (Exception ex)
            {
                throw Errors.ErrorParsingLcsLine(line, ex.Message);
            }
            foreach (var child in line.Children)
            {
                Addons.Add(BsAddon.FromLcs(child));
            }
        }
    }
}
