using System;
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

        public void Deactivate(BsMap map)
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

        public static BsAddon FromLcs(LcsLine line)
        {
            var result = ResourceSystem.GetTemplateAddon(LcsParser.ParseString(line.Header[0]));
            result.ParseLcs(line);
            return result;
        }

        private void ParseLcs(LcsLine line)
        {
            Id = LcsParser.ParseString(line.Header[1]);
            try
            {
                _FromLcs(line.Properties);
            }
            catch (Exception ex)
            {
                throw Errors.InvalidLcsLine(line, ex.Message);
            }
        }
    }
}
