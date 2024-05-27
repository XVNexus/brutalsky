using System.Collections.Generic;
using Data.Logic;
using JetBrains.Annotations;
using Lcs;
using Systems;
using UnityEngine;

namespace Data.Base
{
    public abstract class BsAddon
    {
        public abstract string Tag { get; }
        public string Id { get; set; }

        [CanBeNull] public Component InstanceComponent { get; private set; }
        public bool Active { get; private set; }

        protected BsAddon(string id)
        {
            Id = id;
        }

        protected abstract Component _Init(GameObject gameObject, BsObject parentObject, BsMap map);

        protected abstract object[] _ToLcs();

        protected abstract void _FromLcs(object[] props);

        [CanBeNull]
        public virtual BsNode RegisterLogic()
        {
            return null;
        }

        public void Activate(GameObject gameObject, BsObject parentObject, BsMap map)
        {
            InstanceComponent = _Init(gameObject, parentObject, map);
            Active = true;
        }

        public void Deactivate()
        {
            InstanceComponent = null;
            Active = false;
        }

        public LcsLine ToLcs()
        {
            var props = new List<object> { Tag, Id };
            props.AddRange(_ToLcs());
            return new LcsLine('@', props.ToArray());
        }

        public static BsAddon FromLcs(LcsLine line)
        {
            var result = ResourceSystem.GetTemplateAddon((string)line.Props[0]);
            result.ParseLcs(line);
            return result;
        }

        private void ParseLcs(LcsLine line)
        {
            Id = (string)line.Props[1];
            _FromLcs(line.Props[2..]);
        }

        public override string ToString()
        {
            return $"ADDON :: {Tag}.{Id}";
        }
    }
}
