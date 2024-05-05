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

        protected abstract string[] _ToLcs();

        protected abstract void _FromLcs(string[] properties);

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
            var properties = new List<string> { Stringifier.Str(LcsType.String, Tag), Stringifier.Str(LcsType.String, Id) };
            properties.AddRange(_ToLcs());
            return new LcsLine('@', properties.ToArray());
        }

        public static BsAddon FromLcs(LcsLine line)
        {
            var result = ResourceSystem.GetTemplateAddon(Stringifier.Par<string>(LcsType.String, line.Properties[0]));
            result.ParseLcs(line);
            return result;
        }

        private void ParseLcs(LcsLine line)
        {
            Id = Stringifier.Par<string>(LcsType.String, line.Properties[1]);
            try
            {
                _FromLcs(line.Properties[2..]);
            }
            catch (Exception ex)
            {
                throw Errors.ErrorParsingLcsLine(line, ex.Message);
            }
        }
    }
}
