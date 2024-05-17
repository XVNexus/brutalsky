using System.Collections.Generic;
using Brutalsky.Logic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Lcs;

namespace Brutalsky.Base
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

        protected BsAddon()
        {
        }

        protected abstract Component _Init(GameObject gameObject, BsObject parentObject, BsMap map);

        protected abstract LcsProp[] _ToLcs();

        protected abstract void _FromLcs(LcsProp[] props);

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
            var props = new List<LcsProp>
            {
                new(Tag),
                new(Id)
            };
            props.AddRange(_ToLcs());
            return new LcsLine('@', props.ToArray());
        }

        public static BsAddon FromLcs(LcsLine line)
        {
            var result = ResourceSystem.GetTemplateAddon((string)line.Props[0].Value);
            result.ParseLcs(line);
            return result;
        }

        private void ParseLcs(LcsLine line)
        {
            Id = (string)line.Props[1].Value;
            _FromLcs(line.Props[2..]);
        }

        public override string ToString()
        {
            return $"ADDON :: {Tag}.{Id}";
        }
    }
}
