using System;
using System.Collections.Generic;
using Controllers.Base;
using Extensions;
using JetBrains.Annotations;
using Lcs;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Base
{
    public class BsObject : IHasId, ILcsLine
    {
        public virtual GameObject Prefab { get; set; }
        public virtual string Tag { get; set; }
        public string Id { get; set; }
        public string[] Relatives { get; set; } = Array.Empty<string>();

        public Vector2 Position { get; set; } = Vector2.zero;
        public float Rotation { get; set; }
        public sbyte Layer { get; set; }
        public object[] Props { get; set; }

        [CanBeNull] public virtual Func<BsBehavior, BsNode> GetNode { get; set; } = null;
        public virtual Func<GameObject, BsObject, BsObject[], BsBehavior> Init { get; set; }
        [CanBeNull] public GameObject InstanceObject { get; private set; }
        [CanBeNull] public BsBehavior InstanceController { get; private set; }
        public bool Active { get; private set; }

        public BsObject(string id, string[] relatives)
        {
            Id = id;
            Relatives = relatives;
        }

        public BsObject() { }

        public T As<T>() where T : BsObject, new()
        {
            return new T
            {
                Prefab = Prefab,
                Tag = Tag,
                Id = Id,
                Relatives = Relatives,
                Position = Position,
                Rotation = Rotation,
                Layer = Layer,
                Props = Props,
                Init = Init,
                InstanceObject = InstanceObject,
                InstanceController = InstanceController,
                Active = Active
            };
        }

        public void Activate(Transform parent, BsObject[] relatedObjects)
        {
            if (!parent) throw Errors.ParentObjectUnbuilt();
            InstanceObject = UnityEngine.Object.Instantiate(Prefab, parent);
            InstanceController = Init(InstanceObject, this, relatedObjects);
            Active = true;
        }

        public void Deactivate()
        {
            UnityEngine.Object.Destroy(InstanceObject);
            InstanceObject = null;
            InstanceController = null;
            Active = false;
        }

        public LcsLine _ToLcs()
        {
            var result = new List<object> { Tag, Id, LcsInfo.Convert(Relatives), Position.ToLcs(), Rotation, Layer };
            result.AddRange(Props);
            return new LcsLine('#', result.ToArray());
        }

        public void _FromLcs(LcsLine line)
        {
            Tag = line.Get<string>(0);
            var result = ResourceSystem.GetTemplateObject(Tag);
            Prefab = result.Prefab;
            GetNode = result.GetNode;
            Init = result.Init;
            Id = line.Get<string>(1);
            var rawRelatives = line.Get<object[]>(2);
            Relatives = new string[rawRelatives.Length];
            for (var i = 0; i < rawRelatives.Length; i++)
            {
                Relatives[i] = (string)rawRelatives[i];
            }
            Position = Vector2Ext.FromLcs(line.Get(3));
            Rotation = line.Get<float>(4);
            Layer = line.Get<sbyte>(5);
            Props = line.Props[6..];
        }

        public override string ToString()
        {
            return $"OBJECT: {Tag}:{Id}";
        }
    }
}
