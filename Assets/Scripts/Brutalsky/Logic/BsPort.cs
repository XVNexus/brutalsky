using System;
using Core;
using Utils.Constants;

namespace Brutalsky.Logic
{
    public class BsPort
    {
        public string Id { get => _id; set => _id = MapSystem.CleanId(value); }
        private string _id;

        public float ValueIfChanged
        {
            get
            {
                if (!Changed) return float.NaN;
                Changed = false;
                return _value;
            }
        }
        public float Value
        {
            get
            {
                _value = _get();
                return _value;
            }
            set
            {
                _set(value);
                _value = value;
                Changed = true;
            }
        }
        public bool Changed { get; set; }
        private Func<float> _get;
        private Action<float> _set;
        private float _value;

        // Read/write port
        public BsPort(string parentId, string id, Func<float> get, Action<float> set, bool startChanged = true)
        {
            Id = parentId + '.' + id;
            _get = get;
            _set = set;
            Changed = startChanged;
        }

        // Readonly port
        public BsPort(string parentId, string id, Func<float> get, bool startChanged = true)
        {
            Id = parentId + '.' + id;
            _get = get;
            _set = _ => throw Errors.SetReadOnlyPort(Id);
            Changed = startChanged;
        }

        // Constant port
        public BsPort(string parentId, string id, float value, bool startChanged = true)
        {
            Id = parentId + '.' + id;
            _get = () => _value;
            _set = _ => throw Errors.SetReadOnlyPort(Id);
            _value = value;
            Changed = startChanged;
        }

        // Standalone port (relies on internal value instead of external reference)
        public BsPort(string parentId, string id, bool startChanged = true)
        {
            Id = parentId + '.' + id;
            _get = () => _value;
            _set = value => _value = value;
            Changed = startChanged;
        }
    }
}
