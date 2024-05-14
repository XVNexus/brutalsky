using Utils.Lcs;

namespace Utils.Config
{
    public class ConfigOption
    {
        public string Id { get; }
        public LcsType Type
        {
            get => _prop.Type;
            set => _prop.Type = value;
        }
        public object Value
        {
            get => _prop.Value;
            set => _prop.Value = value;
        }
        private LcsProp _prop;

        public ConfigOption(string id, LcsType type, object value)
        {
            Id = id;
            _prop = new LcsProp(type, value);
        }

        public ConfigOption(string id, LcsProp prop)
        {
            Id = id;
            _prop = prop;
        }

        public LcsLine ToLcs()
        {
            return new LcsLine('$', new[] { new LcsProp(LcsType.String, Id), _prop });
        }

        public static ConfigOption FromLcs(LcsLine line)
        {
            return new ConfigOption((string)line.Props[0].Value, line.Props[1]);
        }
    }
}
