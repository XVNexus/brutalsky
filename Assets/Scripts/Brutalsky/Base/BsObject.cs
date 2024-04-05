using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using Serializable;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
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

        protected abstract Component _Init(GameObject gameObject, BsMap map);

        protected abstract Dictionary<string, string> _ToSrz();

        protected abstract void _FromSrz(Dictionary<string, string> properties);

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

        public SrzObject ToSrz()
        {
            var srzAddons = new List<SrzAddon>();
            foreach (var addon in Addons)
            {
                srzAddons.Add(addon.ToSrz());
            }
            return new SrzObject(Tag, Id, _ToSrz(), srzAddons);
        }

        public void FromSrz(string id, SrzObject srzObject)
        {
            _FromSrz(srzObject.pr);
            foreach (var srzAddon in srzObject.ad)
            {
                var addonIdParts = BsUtils.SplitFullId(srzAddon.id);
                var addonTag = addonIdParts[0];
                var addonId = addonIdParts[1];
                var addon = ResourceSystem._.GetTemplateAddon(addonTag);
                addon.FromSrz(addonId, srzAddon);
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
