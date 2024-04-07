using JetBrains.Annotations;
using Serializable;
using UnityEngine;
using Utils;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsAddon
    {
        public abstract string Tag { get; }
        public string Id { get; set; }
        public ObjectTransform Transform { get; set; }

        [CanBeNull] public Component InstanceComponent { get; set; }
        public bool Active { get; set; }

        protected BsAddon(string id, ObjectTransform transform)
        {
            Id = id;
            Transform = transform;
        }

        protected BsAddon()
        {
        }

        protected abstract Component _Init(GameObject gameObject, BsObject parentObject, BsMap map);

        protected abstract string[] _ToSrz();

        protected abstract void _FromSrz(string[] properties);

        public Component Init(GameObject gameObject, BsObject parentObject, BsMap map)
        {
            return _Init(gameObject, parentObject, map);
        }

        public SrzAddon ToSrz()
        {
            return new SrzAddon(Tag, Id, _ToSrz());
        }

        public void FromSrz(string id, SrzAddon srzAddon)
        {
            Id = id;
            _FromSrz(SrzUtils.ExpandProperties(srzAddon.pr));
        }

        public void Activate(Component instanceComponent)
        {
            InstanceComponent = instanceComponent;
            Active = true;
        }

        public void Deactivate()
        {
            InstanceComponent = null;
            Active = false;
        }
    }
}
