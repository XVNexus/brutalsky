using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Utils.Lcs
{
    public struct LcsLine
    {
        public char Prefix { get; set; }
        public object[] Props { get; set; }
        public List<LcsLine> Children { get; set; }

        public LcsLine(char prefix, object[] props, [CanBeNull] List<LcsLine> children = null)
        {
            Prefix = prefix;
            Props = props;
            Children = children ?? new List<LcsLine>();
        }

        public LcsLine(char prefix, params object[] props)
        {
            Prefix = prefix;
            Props = props;
            Children = new List<LcsLine>();
        }

        public byte[] Binify()
        {
            var result = new List<byte> { (byte)Prefix };
            foreach (var prop in Props)
            {
                result.AddRange(LcsInfo.Binify(prop));
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
            var props = new List<object>();
            while (cursor < raw.Length)
            {
                if (raw[cursor] == 0) break;
                props.Add(LcsInfo.Parse(raw, ref cursor));
            }
            return new LcsLine(prefix, props.ToArray());
        }

        public string Stringify()
        {
            return Children.Aggregate($"{Prefix} " +
                $"{LcsInfo.ConcatProps(Props.Select(LcsInfo.Stringify).ToArray())}\n",
                (current, child) => current + child.Stringify());
        }

        public static LcsLine Parse(string raw)
        {
            return new LcsLine(raw[0], LcsInfo.SplitProps(raw[2..]).Select(LcsInfo.Parse).ToArray());
        }

        public override string ToString()
        {
            return Props.Aggregate("[[ ", (current, prop) => current + $"{prop} ") + "]]";
        }
    }
}
