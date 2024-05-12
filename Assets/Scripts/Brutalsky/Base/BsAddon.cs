using System;
using System.Collections.Generic;
using Brutalsky.Logic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Base
{
    public abstract class BsAddon
    {
        public abstract string Tag { get; }
        public abstract bool HasLogic { get; }
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
                new(LcsType.String, Tag),
                new(LcsType.String, Id),
                new(LcsType.Transform, Transform)
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
            Transform = (ObjectTransform)line.Props[2].Value;
            _FromLcs(line.Props[3..]);
        }
    }
}
