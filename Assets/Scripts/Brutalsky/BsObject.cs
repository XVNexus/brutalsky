using JetBrains.Annotations;
using UnityEngine;
using EventSystem = Core.EventSystem;

namespace Brutalsky
{
    public abstract class BsObject
    {
        public uint id { get; } = EventSystem.random.NextUInt();
        
        public BsTransform transform { get; set; }
        [CanBeNull] public GameObject instanceObject { get; set; }
        [CanBeNull] public Component instanceComponent { get; set; }
        public bool active { get; set; }

        public abstract char saveSymbol { get; }

        public abstract void Parse(string[][] raw);

        public abstract string[][] Stringify();
    }
}
