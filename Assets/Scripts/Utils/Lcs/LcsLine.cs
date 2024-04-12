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
            var parts = raw[1..].Trim().Split(LcsParser.HeaderSeparator);
            var header = LcsParser.ExpandProperties(parts[0]);
            var properties = LcsParser.ExpandProperties(parts[1]);
            return new LcsLine(prefix, header, properties);
        }

        public string Stringify()
        {
            var result = $"{Prefix}{LcsParser.CompressProperties(Header)}:{LcsParser.CompressProperties(Properties)}\n";
            result = Children.Aggregate(result, (current, child) => current + child.Stringify());
            return result;
        }
    }
}
