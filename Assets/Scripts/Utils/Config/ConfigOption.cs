using Lcs;

namespace Utils.Config
{
    public class ConfigOption
    {
        public string Id { get; }
        public object Value { get; set; }

        public ConfigOption(string id, object value)
        {
            Id = id;
            Value = value;
        }

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

        public LcsLine ToLcs()
        {
            return new LcsLine('$', Id, Value);
        }

        public static ConfigOption FromLcs(LcsLine line)
        {
            return new ConfigOption((string)line.Props[0], line.Props[1]);
        }
    }
}
