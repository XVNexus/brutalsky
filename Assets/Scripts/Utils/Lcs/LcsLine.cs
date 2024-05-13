using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Utils.Lcs
{
    public class LcsLine
    {
        public char Prefix { get; set; }
        public LcsProp[] Props { get; set; }
        public List<LcsLine> Children { get; set; }

        public LcsLine(char prefix, LcsProp[] props, [CanBeNull] List<LcsLine> children = null)
        {
            Prefix = prefix;
            Props = props;
            Children = children ?? new List<LcsLine>();
        }

        public byte[] Binify()
        {
            var result = new List<byte> { (byte)Prefix };
            foreach (var prop in Props)
            {
                result.AddRange(prop.Binify());
            }
            result.Add(0);
            foreach (var child in Children)
            {
                result.AddRange(child.Binify());
            }
            return result.ToArray();
        }

        public static LcsLine Parse(byte[] raw, ref int cursor)
        {
            var prefix = (char)raw[cursor++];
            var props = new List<LcsProp>();
            while (cursor < raw.Length)
            {
                if (raw[cursor] == 0) break;
                props.Add(LcsProp.Parse(raw, ref cursor));
            }
            return new LcsLine(prefix, props.ToArray());
        }

        public string Stringify()
        {
            return Children.Aggregate($"{Prefix}{LcsInfo.PrefixSeparator}" +
                $"{LcsInfo.ConcatProps(Props.Select(prop => prop.Stringify()).ToArray())}{LcsInfo.LineSeparator}",
                (current, child) => current + child.Stringify());
        }

        public static LcsLine Parse(string raw)
        {
            return new LcsLine(raw[0], LcsInfo.SplitProps(raw[(LcsInfo.PrefixSeparator.Length + 1)..])
                .Select(prop => LcsProp.Parse(prop)).ToArray());
        }

        public override string ToString()
        {
            return Props.Aggregate("[[   ", (current, prop) => current + $"{prop}   ") + "]]";
        }
    }
}
