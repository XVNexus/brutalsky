using Brutalsky.Logic;
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

        protected BsAddon(string id, ObjectTransform transform)
        {
            Id = id;
            Transform = transform;
        }

        protected BsAddon()
        {
        }

        protected abstract Component _Init(GameObject gameObject, BsObject parentObject, BsMap map);

        [CanBeNull]
        protected virtual BsNode RegisterLogic()
        {
            return null;
        }

        protected abstract string[] _ToLcs();

        protected abstract void _FromLcs(string[] properties);

        public void Activate(GameObject gameObject, BsObject parentObject, BsMap map)
        {
            InstanceComponent = _Init(gameObject, parentObject, map);
            Active = true;
            var logicNode = RegisterLogic();
            if (logicNode != null)
            {
                map.Matrix.AddNode(logicNode);
            }
        }

        public void Deactivate()
        {
            InstanceComponent = null;
            Active = false;
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
    }
}
