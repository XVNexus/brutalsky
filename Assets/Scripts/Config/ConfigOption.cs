using JetBrains.Annotations;
using Lcs;
using Utils;

namespace Config
{
    public class ConfigOption : IHasId, ILcsLine
    {
        public string Id { get; set; } = "";
        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                Changed = true;
            }
        }
        private object _value = false;
        public bool Changed { get; private set; } = true;

        public ConfigOption(string id, object value)
        {
            Id = id;
            Value = value;
        }

        public ConfigOption() { }

        [CanBeNull]
        public object GetIfChanged()
        {
            var result = Changed ? Value : null;
            Changed = false;
            return result;
        }

        public string Stringify()
        {
            return LcsInfo.Stringify(Value);
        }

        public bool Parse(string raw)
        {
            try
            {
                var newValue = LcsInfo.Parse(raw);
                if (newValue != _value)
                {
                    Value = newValue;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public LcsLine _ToLcs()
        {
            return new LcsLine('$', Id, Value);
        }

        public void _FromLcs(LcsLine line)
        {
            Id = line.Get<string>(0);
            Value = line.Get(1);
        }
    }
}
