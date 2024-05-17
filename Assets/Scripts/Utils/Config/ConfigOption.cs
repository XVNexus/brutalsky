using Utils.Lcs;

namespace Utils.Config
{
    public class ConfigOption
    {
        public string Id { get; }
        public object Value
        {
            get => _prop.Value;
            set => _prop.Value = value;
        }

        private LcsProp _prop;

        public ConfigOption(string id, object value)
        {
            Id = id;
            _prop = new LcsProp(value);
        }

        public ConfigOption(string id, LcsProp prop)
        {
            Id = id;
            _prop = prop;
        }

        public string Stringify()
        {
            return _prop.Stringify();
        }

        public bool Parse(string raw)
        {
            try
            {
                Value = LcsProp.Parse(raw);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public LcsLine ToLcs()
        {
            return new LcsLine('$', new[] { new LcsProp(Id), _prop });
        }

        public static ConfigOption FromLcs(LcsLine line)
        {
            return new ConfigOption((string)line.Props[0].Value, line.Props[1]);
        }
    }
}
