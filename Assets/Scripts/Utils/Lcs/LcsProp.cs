using System.Collections.Generic;
using System.Linq;

namespace Utils.Lcs
{
    public struct LcsProp
    {
        public LcsType Type { get; set; }
        public object Value { get; set; }

        public LcsProp(LcsType type, object value)
        {
            Type = type;
            Value = value;
        }

        public byte[] Binify()
        {
            var typeInfo = LcsInfo.TypeTable[Type];
            var typeBytes = new[] { typeInfo.ByteTag };
            var valueBytes = typeInfo.ToBin(Value);
            var size = typeInfo.Size;
            if (size > -1) return typeBytes.Concat(valueBytes).ToArray();
            var headerBytes = new List<byte>();
            var byteCount = valueBytes.Length;
            while (byteCount > 127)
            {
                headerBytes.Add((byte)(byteCount | 0x80));
                byteCount /= 128;
            }
            headerBytes.Add((byte)byteCount);
            return typeBytes.Concat(headerBytes).Concat(valueBytes).ToArray();
        }

        public static LcsProp Parse(byte[] raw, ref int cursor)
        {
            var type = LcsInfo.ByteTagTable[raw[cursor++]];
            var typeInfo = LcsInfo.TypeTable[type];
            var size = typeInfo.Size;
            if (size == -1)
            {
                size = 0;
                var power = 1;
                while ((raw[cursor] & 0x80) > 0)
                {
                    size += (raw[cursor++] & 0x7F) * power; 
                    power *= 128;
                }
                size += raw[cursor++] * power;
            }
            var valueBytes = new byte[size];
            for (var i = 0; i < size; i++)
            {
                valueBytes[i] = raw[cursor++];
            }
            var value = typeInfo.FromBin(valueBytes);
            return new LcsProp(type, value);
        }

        public string Stringify()
        {
            var typeInfo = LcsInfo.TypeTable[Type];
            return $"{typeInfo.StringTag}{LcsInfo.TypeSeparator}{typeInfo.ToStr(Value)}";
        }

        public static LcsProp Parse(string raw)
        {
            var parts = raw.Split(LcsInfo.TypeSeparator);
            var type = LcsInfo.StringTagTable[parts[0]];
            var typeInfo = LcsInfo.TypeTable[type];
            var value = typeInfo.FromStr(parts[1]);
            return new LcsProp(type, value);
        }

        public override string ToString()
        {
            return $"({Type}: {Value})";
        }
    }
}
