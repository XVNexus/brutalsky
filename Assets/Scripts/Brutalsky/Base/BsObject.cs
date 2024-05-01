using System.Collections.Generic;
using System.Linq;
using Brutalsky.Map;
using Controllers.Base;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsObject
    {
        public List<BsAddon> Addons { get; } = new();
        public abstract GameObject Prefab { get; }
        public abstract char Tag { get; }
        public string Id { get => _id; set => _id = MapSystem.CleanId(value); }
        private string _id;
        public ObjectTransform Transform { get; set; }
        public ObjectLayer Layer { get; set; }
        public bool Simulated { get; set; }

        [CanBeNull] public GameObject InstanceObject { get; private set; }
        [CanBeNull] public BsBehavior InstanceController { get; private set; }
        public bool Active { get; private set; }
        public Dictionary<string, BsProp> Props { get; private set; }

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

        protected abstract string[] _ToLcs();

        protected abstract void _FromLcs(string[] properties);

        public void Activate(Transform parent, BsMap map)
        {
            InstanceObject = UnityEngine.Object.Instantiate(Prefab, parent);
            InstanceController = _Init(InstanceObject, map);
            Active = true;
            foreach (var addon in Addons)
            {
                var instanceComponent = addon.Init(InstanceObject, this, map);
                addon.Activate(instanceComponent);
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

        public void FromLcs(LcsLine line)
        {
            Id = LcsParser.ParseString(line.Header[1]);
            _FromLcs(line.Properties);
            foreach (var child in line.Children)
            {
                var addon = ResourceSystem._.GetTemplateAddon(LcsParser.ParseChar(child.Header[0]));
                addon.FromLcs(child);
                Addons.Add(addon);
            }
        }
    }
}
