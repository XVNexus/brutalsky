using System.Collections.Generic;
using System.Linq;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsObject
    {
        public List<BsAddon> Addons { get; } = new();
        public abstract GameObject Prefab { get; }
        public abstract char Tag { get; }
        public string Id { get => _id; set => _id = BsUtils.CleanId(value); }
        private string _id;
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

        protected abstract Component _Init(GameObject gameObject, BsMap map);

        protected abstract string[] _ToLcs();

        protected abstract void _FromLcs(string[] properties);

        public Component Init(GameObject gameObject, BsMap map)
        {
            var instanceController = _Init(gameObject, map);
            foreach (var addon in Addons)
            {
                var instanceComponent = addon.Init(gameObject, this, map);
                addon.Activate(instanceComponent);
            }
            return instanceController;
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

        public void Activate(GameObject instanceObject, Component instanceController)
        {
            InstanceObject = instanceObject;
            InstanceController = instanceController;
            Active = true;
        }

        public void Deactivate()
        {
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
