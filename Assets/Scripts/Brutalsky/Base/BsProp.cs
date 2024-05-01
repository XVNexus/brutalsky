using System;
using Utils.Constants;

namespace Brutalsky.Base
{
    public class BsProp
    {
        public string Id { get; private set; }
        public object Reference { get; private set; }
        public bool ReadOnly { get; private set; }

        private Func<object, float> _get;
        private Action<object, float> _set;

        public BsProp(string id, object reference, Func<object, float> get, Action<object, float> set)
        {
            Id = id;
            Reference = reference;
            _get = get;
            _set = set;
            ReadOnly = false;
        }

        public BsProp(string id, object reference, Func<object, float> get)
        {
            Id = id;
            Reference = reference;
            _get = get;
            ReadOnly = true;
        }

        public float GetValue()
        {
            return _get(Reference);
        }

        public void SetValue(float value)
        {
            if (ReadOnly) throw Errors.WriteReadOnlyProp();
            _set(Reference, value);
        }
    }
}
