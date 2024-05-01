using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsAddon
    {
        public abstract char Tag { get; }
        public string Id { get => _id; set => _id = MapSystem.CleanId(value); }
        private string _id;
        public ObjectTransform Transform { get; set; }

        [CanBeNull] public Component InstanceComponent { get; private set; }
        public bool Active { get; private set; }
        public Dictionary<string, BsProp> Props { get; private set; }

        protected BsAddon(string id, ObjectTransform transform)
        {
            Id = id;
            Transform = transform;
        }

        protected BsAddon()
        {
        }

        protected abstract Component _Init(GameObject gameObject, BsObject parentObject, BsMap map);

        protected abstract string[] _ToLcs();

        protected abstract void _FromLcs(string[] properties);

        public Component Init(GameObject gameObject, BsObject parentObject, BsMap map)
        {
            return _Init(gameObject, parentObject, map);
        }

        public LcsLine ToLcs()
        {
            return new LcsLine
            (
                '@',
                new[] { LcsParser.Stringify(Tag), LcsParser.Stringify(Id) },
                _ToLcs()
            );
        }

        public void FromLcs(LcsLine line)
        {
            Id = LcsParser.ParseString(line.Header[1]);
            _FromLcs(line.Properties);
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
