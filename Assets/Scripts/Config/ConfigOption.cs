using Lcs;
using Utils;

namespace Config
{
    public class ConfigOption : ILcsLine, IHasId
    {
        public string Id { get; set; } = "";
        public object Value { get; set; } = false;

        public ConfigOption(string id, object value)
        {
            Id = id;
            Value = value;
        }

        public ConfigOption() { }

        public string Stringify()
        {
            return LcsInfo.Stringify(Value);
        }

        public bool Parse(string raw)
        {
            try
            {
                Value = LcsInfo.Parse(raw);
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
            Id = (string)line.Props[0];
            Value = line.Props[1];
        }
    }
}
