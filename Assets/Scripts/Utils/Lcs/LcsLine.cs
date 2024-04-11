using System.Collections.Generic;
using System.Linq;
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
            var parts = raw[1..].Trim().Split(SrzUtils.HeaderSeparator);
            var header = SrzUtils.ExpandProperties(parts[0]);
            var properties = SrzUtils.ExpandProperties(parts[1]);
            return new LcsLine(prefix, header, properties);
        }

        public string Stringify()
        {
            var result = $"{Prefix}{SrzUtils.CompressProperties(Header)}:{SrzUtils.CompressProperties(Properties)}\n";
            result = Children.Aggregate(result, (current, child) => current + child.Stringify());
            return result;
        }
    }
}
