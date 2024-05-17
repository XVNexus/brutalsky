using System.Collections.Generic;
using System.Linq;

namespace Utils.Lcs
{
    public struct LcsProp
    {
        public object Value { get; set; }

        public LcsProp(object value)
        {
            Value = value;
        }

        public byte[] Binify()
        {
            var typeInfo = LcsInfo.GetTypeInfo(Value);
            var typeBytes = new[] { typeInfo.ByteTag };
            var valueBytes = typeInfo.Binify(Value);
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
            var typeInfo = LcsInfo.ByteTagTable[raw[cursor++]];
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
            return new LcsProp(typeInfo.Parse(valueBytes));
        }

        public string Stringify()
        {
            return LcsInfo.GetTypeInfo(Value).Stringify(Value);
        }

        public static LcsProp Parse(string raw)
        {
            return new LcsProp(LcsInfo.TypeInfoList.First(type => type.StringPattern.IsMatch(raw)).Parse(raw));
        }

        public override string ToString()
        {
            return $"({Value})";
        }
    }
}
