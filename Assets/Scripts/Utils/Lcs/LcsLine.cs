using System;
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

        public static LcsLine Parse(byte[] raw)
        {
            var props = new List<LcsProp>();
            var cursor = 1;
            while (cursor < raw.Length)
            {
                var propType = (LcsType)raw[cursor];
                var byteCount = LcsProp.TypeByteCountTable[propType];
                if (byteCount == 0)
                {
                    byteCount = raw[cursor + 1];
                    cursor++;
                }
                var propBytes = new byte[byteCount];
                for (var i = 0; i < byteCount; i++)
                {
                    propBytes[i] = raw[cursor];
                    cursor++;
                }
                props.Add(LcsProp.Parse(propType, propBytes.ToArray()));
                cursor++;
            }
            return new LcsLine((char)raw[0], props.ToArray());
        }

        public string Stringify()
        {
            return Children.Aggregate(
                $"{Prefix}{Stringifier.CompressProps(Props.Select(prop => prop.Stringify()).ToArray())}\n",
                (current, child) => current + child.Stringify());
        }

        public static LcsLine Parse(string raw)
        {
            return new LcsLine(raw[0],
                Stringifier.ExpandProps(raw[1..]).Select(prop => LcsProp.Parse(prop)).ToArray());
        }

        public override string ToString()
        {
            return Stringify().TrimEnd();
        }
    }
}
