using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Utils.Lcs
{
    public class LcsLine
    {
        public char Prefix { get; set; }
        public string[] Properties { get; set; }
        public List<LcsLine> Children { get; set; }

        public LcsLine(char prefix, string[] properties, [CanBeNull] List<LcsLine> children = null)
        {
            Prefix = prefix;
            Properties = properties;
            Children = children ?? new List<LcsLine>();
        }

        public static LcsLine Parse(string raw)
        {
            var prefix = raw[0];
            var properties = LcsParser.ExpandProperties(raw[1..]);
            return new LcsLine(prefix, properties);
        }

        public string Stringify()
        {
            var result = $"{Prefix}{LcsParser.CompressProperties(Properties)}\n";
            result = Children.Aggregate(result, (current, child) => current + child.Stringify());
            return result;
        }

        public override string ToString()
        {
            return $"{Prefix}{LcsParser.CompressProperties(Properties)}";
        }
    }
}
