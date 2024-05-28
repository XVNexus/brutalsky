using System;
using System.Collections.Generic;
using Data.Logic;
using JetBrains.Annotations;
using Lcs;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Base
{
    public abstract class BsAddon : IHasId
    {
        public abstract string Tag { get; }
        public string Id { get; set; }
        public string[] Relatives { get; set; } = Array.Empty<string>();

        [CanBeNull] public Component InstanceComponent { get; private set; }
        public bool Active { get; private set; }

        protected BsAddon(string id, params string[] relatives)
        {
            Id = id;
            Relatives = relatives;
        }

        protected BsAddon(string id)
        {
            Id = id;
        }

        protected abstract Component _Init(GameObject gameObject, BsObject parentObject, BsObject[] relatedObjects);

        [CanBeNull]
        public virtual BsNode RegisterLogic()
        {
            return null;
        }

        public void Activate(GameObject gameObject, BsObject parentObject, BsObject[] relatedObjects)
        {
            InstanceComponent = _Init(gameObject, parentObject, relatedObjects);
            Active = true;
        }

        public void Deactivate()
        {
            InstanceComponent = null;
            Active = false;
        }
    }
}
