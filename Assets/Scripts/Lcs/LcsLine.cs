using System.Collections.Generic;
using System.Linq;

namespace Lcs
{
    public struct LcsLine
    {
        public char Prefix { get; }
        // Only supports primitive datatypes (bool, byte, ushort, uint, ulong, sbyte, short, int, long, float, double, char, string)
        // and object arrays with items of primitive datatypes
        public object[] Props { get; }
        public List<LcsLine> Children { get; }

        public LcsLine(char prefix, object[] props, List<LcsLine> children = null)
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

        public static LcsLine Serialize<T>(T value) where T : ILcsLine, new()
        {
            return value._ToLcs();
        }

        public static T Deserialize<T>(LcsLine line) where T : ILcsLine, new()
        {
            var result = new T();
            result._FromLcs(line);
            return result;
        }

        public IEnumerable<byte> Binify()
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
            return result;
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
            return Children.Aggregate($"{Prefix} " + $"{string.Join(' ', Props.Select(LcsInfo.Stringify).ToArray())}\n",
                (current, child) => current + child.Stringify());
        }

        public static LcsLine Parse(string raw)
        {
            return new LcsLine(raw[0], raw[2..].Split(' ').Select(LcsInfo.Parse).ToArray());
        }
    }
}
