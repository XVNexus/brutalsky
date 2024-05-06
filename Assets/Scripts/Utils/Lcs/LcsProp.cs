using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Lcs
{
    public class LcsProp
    {
        public static readonly Dictionary<LcsType, int> TypeByteCountTable = new()
        {
            { LcsType.Bool, 1 },
            { LcsType.UShort, 2 },
            { LcsType.UInt, 4 },
            { LcsType.ULong, 8 },
            { LcsType.Short, 2 },
            { LcsType.Int, 4 },
            { LcsType.Long, 8 },
            { LcsType.Float, 4 },
            { LcsType.Double, 8 },
            { LcsType.Char, 1 },
            { LcsType.String, 0 },
            { LcsType.Direction, 1 },
            { LcsType.Layer, 1 },
            { LcsType.FormType, 1 },
            { LcsType.JointType, 1 },
            { LcsType.Vector2, 8 },
            { LcsType.Color, 4 },
            { LcsType.Transform, 12 },
            { LcsType.Form, 0 },
            { LcsType.Material, 20 },
            { LcsType.Chemical, 12 }
        };
        public static readonly Dictionary<LcsType, char> TypeCharTable = new()
        {
            { LcsType.Bool, 'b' },
            { LcsType.UShort, 'h' },
            { LcsType.UInt, 'n' },
            { LcsType.ULong, 'o' },
            { LcsType.Short, 's' },
            { LcsType.Int, 'i' },
            { LcsType.Long, 'l' },
            { LcsType.Float, 'f' },
            { LcsType.Double, 'd' },
            { LcsType.Char, 'c' },
            { LcsType.String, 't' },
            { LcsType.Direction, 'D' },
            { LcsType.Layer, 'L' },
            { LcsType.FormType, 'F' },
            { LcsType.JointType, 'J' },
            { LcsType.Vector2, 'V' },
            { LcsType.Color, 'C' },
            { LcsType.Transform, 'T' },
            { LcsType.Form, 'O' },
            { LcsType.Material, 'M' },
            { LcsType.Chemical, 'H' }
        };
        public static readonly Dictionary<char, LcsType> CharTypeTable = new()
        {
            { 'b', LcsType.Bool },
            { 'h', LcsType.UShort },
            { 'n', LcsType.UInt },
            { 'o', LcsType.ULong },
            { 's', LcsType.Short },
            { 'i', LcsType.Int },
            { 'l', LcsType.Long },
            { 'f', LcsType.Float },
            { 'd', LcsType.Double },
            { 'c', LcsType.Char },
            { 't', LcsType.String },
            { 'D', LcsType.Direction },
            { 'L', LcsType.Layer },
            { 'F', LcsType.FormType },
            { 'J', LcsType.JointType },
            { 'V', LcsType.Vector2 },
            { 'C', LcsType.Color },
            { 'T', LcsType.Transform },
            { 'O', LcsType.Form },
            { 'M', LcsType.Material },
            { 'H', LcsType.Chemical }
        };

        public LcsType Type { get; set; }
        public object Value { get; set; }

        public LcsProp(LcsType type, object value)
        {
            Type = type;
            Value = value;
        }

        public byte[] Binify()
        {
            var typeBytes = new[] { (byte)Type };
            var valueBytes = Binifier.Binify(Type, Value);
            var expectedByteCount = TypeByteCountTable[Type];
            if (expectedByteCount > 0) return typeBytes.Concat(valueBytes).ToArray();
            var byteCount = valueBytes.Length;
            if (byteCount > 255) throw new NotImplementedException();
            var headerBytes = new[] { (byte)byteCount };
            return typeBytes.Concat(headerBytes).Concat(valueBytes).ToArray();
        }

        public static LcsProp Parse(byte[] raw, ref int cursor)
        {
            var type = (LcsType)raw[cursor];
            var byteCount = TypeByteCountTable[type];
            if (byteCount == 0)
            {
                byteCount = raw[++cursor];
            }
            cursor++;
            var valueBytes = new byte[byteCount];
            for (var i = 0; i < byteCount; i++)
            {
                valueBytes[i] = raw[cursor++];
            }
            var value = Binifier.Parse(type, valueBytes);
            return new LcsProp(type, value);
        }

        public string Stringify()
        {
            return $"{TypeCharTable[Type]}{Stringifier.Stringify(Type, Value)}";
        }

        public static LcsProp Parse(string raw)
        {
            var type = CharTypeTable[raw[0]];
            var value = Stringifier.Parse(type, raw[1..]);
            return new LcsProp(type, value);
        }

        public override string ToString()
        {
            return $"( {Type} : {Value} )";
        }
    }
}
