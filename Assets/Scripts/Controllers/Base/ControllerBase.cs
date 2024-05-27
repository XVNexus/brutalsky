using System.Collections.Generic;
using Data.Base;
using JetBrains.Annotations;
using Utils.Constants;

namespace Controllers.Base
{
    public abstract class ControllerBase<T> : BsBehavior where T : BsObject
    {
        public abstract string Tag { get; }

        public T Object { get; set; }
        public Dictionary<string, SubControllerBase<T>> Subs = new();

        [CanBeNull]
        public TC GetSub<TC>(string id) where TC : SubControllerBase<T>
        {
            return (TC)Subs.GetValueOrDefault(id);
        }

        public TC RequireSub<TC>(string id, string callerId) where TC : SubControllerBase<T>
        {
            if (!Subs.ContainsKey(id)) throw Errors.MissingSubController(Tag, callerId, id);
            return (TC)Subs[id];
        }

        public bool AddSub(SubControllerBase<T> sub)
        {
            if (ContainsSub(sub)) return false;
            Subs[sub.Id] = sub;
            return true;
        }

        public bool RemoveSub(SubControllerBase<T> sub)
        {
            return RemoveSub(sub.Id);
        }

        public bool RemoveSub(string id)
        {
            if (!ContainsSub(id)) return false;
            Subs.Remove(id);
            return true;
        }

        public bool ContainsSub(SubControllerBase<T> sub)
        {
            return ContainsSub(sub.Id);
        }

        public bool ContainsSub(string id)
        {
            return Subs.ContainsKey(id);
        }
    }
}
