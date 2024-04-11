using System.Collections.Generic;
using JetBrains.Annotations;

namespace Utils.Lcs
{
    public class LcsLine
    {
        public char Prefix { get; set; }
        public string[] Header { get; set; }
        public string[] Properties { get; set; }
        // TODO: MAKE CHILDREN WORK
        public List<LcsLine> Children { get; set; }

        public LcsLine(char prefix, string[] header, string[] properties, [CanBeNull] List<LcsLine> children = null)
        {
            Prefix = prefix;
            Header = header;
            Properties = properties;
            Children = children ?? new List<LcsLine>();
        }

        public static LcsLine Parse(string raw)
        {
            var prefix = raw[0];
            var parts = raw[1..].Split(SrzUtils.HeaderSeparator);
            var header = SrzUtils.ExpandProperties(parts[0]);
            var properties = SrzUtils.ExpandProperties(parts[1]);
            return new LcsLine(prefix, header, properties);
        }

        public string Stringify()
        {
            return $"{Prefix}{SrzUtils.CompressProperties(Header)}:{SrzUtils.CompressProperties(Properties)}";
        }
    }
}
