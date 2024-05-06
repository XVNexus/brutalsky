using System.Collections.Generic;
using Brutalsky.Base;
using JetBrains.Annotations;
using Utils.Constants;

namespace Controllers.Base
{
    public abstract class ControllerBase<TO> : BsBehavior where TO : BsObject
    {
        public abstract string Tag { get; }

        public TO Object { get; set; }
        public Dictionary<string, SubControllerBase<TO>> Subs = new();

        [CanBeNull]
        public TC GetSub<TC>(string id) where TC : SubControllerBase<TO>
        {
            return (TC)Subs.GetValueOrDefault(id);
        }

        public TC RequireSub<TC>(string id, string callerId) where TC : SubControllerBase<TO>
        {
            if (!Subs.ContainsKey(id)) throw Errors.MissingSubController(Tag, callerId, id);
            return (TC)Subs[id];
        }

        public bool AddSub(SubControllerBase<TO> sub)
        {
            if (ContainsSub(sub)) return false;
            Subs[sub.Id] = sub;
            return true;
        }

        public bool RemoveSub(SubControllerBase<TO> sub)
        {
            return RemoveSub(sub.Id);
        }

        public bool RemoveSub(string id)
        {
            if (!ContainsSub(id)) return false;
            Subs.Remove(id);
            return true;
        }

        public bool ContainsSub(SubControllerBase<TO> sub)
        {
            return ContainsSub(sub.Id);
        }

        public bool ContainsSub(string id)
        {
            return Subs.ContainsKey(id);
        }
    }
}
