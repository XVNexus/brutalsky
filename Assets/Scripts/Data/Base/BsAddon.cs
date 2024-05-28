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
    }
}
