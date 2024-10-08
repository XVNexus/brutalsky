using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace Lcs
{
    public struct LcsInfo
    {
        public static readonly Dictionary<char, char> SpecialChars = new()
        {
            {'\'', 'q'},
            {'"', 'u'},
            {',', 'i'},
            {' ', 's'},
            {'\t', 't'},
            {'\n', 'n'}
        };
        public static readonly Dictionary<char, char> SpecialCodes =
            SpecialChars.ToDictionary(kv => kv.Value, kv => kv.Key);

        public static readonly List<LcsInfo> TypeInfoList = new()
        {
            BoolInfo(), ByteInfo(), UShortInfo(), UIntInfo(), ULongInfo(), SByteInfo(), ShortInfo(), IntInfo(), LongInfo(),
            FloatInfo(), DoubleInfo(), CharInfo(), StringInfo(), ArrayInfo()
        };
        public static readonly Dictionary<byte, LcsInfo> ByteTagTable =
            TypeInfoList.ToDictionary(type => type.ByteTag, type => type);
        public static readonly Dictionary<char, LcsInfo> CharTagTable =
            TypeInfoList.ToDictionary(type => type.CharTag, type => type);

        public int Size { get; }
        public byte ByteTag { get; }
        public char CharTag { get; }

        private Func<object, byte[]> _toBin;
        private Func<byte[], object> _fromBin;
        private Func<object, string> _toStr;
        private Func<string, object> _fromStr;

        public LcsInfo(int size, byte byteTag, char charTag,
            Func<object, byte[]> toBin, Func<byte[], object> fromBin,
            Func<object, string> toStr, Func<string, object> fromStr)
        {
            Size = size;
            ByteTag = byteTag;
            CharTag = charTag;
            _toBin = toBin;
            _fromBin = fromBin;
            _toStr = toStr;
            _fromStr = fromStr;
        }

        public static object[] Compress(params object[] values)
        {
            return values;
        }

        public static object[] Convert(IList array)
        {
            var result = new object[array.Count];
            for (var i = 0; i < array.Count; i++)
            {
                result[i] = array[i];
            }
            return result;
        }

        public static LcsInfo GetTypeInfo(object value)
        {
            return value switch
            {
                bool => BoolInfo(),
                byte => ByteInfo(),
                ushort => UShortInfo(),
                uint => UIntInfo(),
                ulong => ULongInfo(),
                sbyte => SByteInfo(),
                short => ShortInfo(),
                int => IntInfo(),
                long => LongInfo(),
                float => FloatInfo(),
                double => DoubleInfo(),
                char => CharInfo(),
                string => StringInfo(),
                object[] => ArrayInfo(),
                _ => throw Errors.InvalidItem("lcs prop", value)
            };
        }

        public static IEnumerable<byte> Binify(object value)
        {
            var typeInfo = GetTypeInfo(value);
            var typeBytes = new[] { typeInfo.ByteTag };
            var valueBytes = typeInfo._toBin(value);
            var size = typeInfo.Size;
            return size > -1
                ? typeBytes.Concat(valueBytes).ToArray()
                : typeBytes.Concat(WriteLengthBytes(valueBytes.Length)).Concat(valueBytes).ToArray();
        }

        public static object Parse(byte[] raw, ref int cursor)
        {
            var typeInfo = ByteTagTable[raw[cursor++]];
            var size = typeInfo.Size;
            if (size == -1)
            {
                size = ReadLengthBytes(raw, ref cursor);
            }
            var valueBytes = new byte[size];
            for (var i = 0; i < size; i++)
            {
                valueBytes[i] = raw[cursor++];
            }
            return typeInfo._fromBin(valueBytes);
        }

        public static string Stringify(object value)
        {
            return GetTypeInfo(value)._toStr(value);
        }

        public static object Parse(string raw)
        {
            return CharTagTable[raw[^1]]._fromStr(raw);
        }

        public static LcsInfo BoolInfo() {
            return new LcsInfo(1, 0x01, 'e',
                value => BitConverter.GetBytes((bool)value),
                raw => BitConverter.ToBoolean(raw),
                value => value.ToString(),
                raw => bool.Parse(raw));
        }

        public static LcsInfo ByteInfo() {
            return new LcsInfo(1, 0x02, 'B',
                value => new[] { (byte)value },
                raw => raw[0],
                value => value.ToString() + 'B',
                raw => byte.Parse(raw[..^1]));
        }

        public static LcsInfo UShortInfo() {
            return new LcsInfo(2, 0x03, 'S',
                value => BitConverter.GetBytes((ushort)value),
                raw => BitConverter.ToUInt16(raw),
                value => value.ToString() + 'S',
                raw => ushort.Parse(raw[..^1]));
        }

        public static LcsInfo UIntInfo() {
            return new LcsInfo(4, 0x04, 'I',
                value => BitConverter.GetBytes((uint)value),
                raw => BitConverter.ToUInt32(raw),
                value => value.ToString() + 'I',
                raw => uint.Parse(raw[..^1]));
        }

        public static LcsInfo ULongInfo() {
            return new LcsInfo(8, 0x05, 'L',
                value => BitConverter.GetBytes((ulong)value),
                raw => BitConverter.ToUInt64(raw),
                value => value.ToString() + 'L',
                raw => ulong.Parse(raw[..^1]));
        }

        public static LcsInfo SByteInfo() {
            return new LcsInfo(1, 0x06, 'b',
                value => new[] { (byte)(sbyte)value },
                raw => (sbyte)raw[0],
                value => value.ToString() + 'b',
                raw => sbyte.Parse(raw[..^1]));
        }

        public static LcsInfo ShortInfo() {
            return new LcsInfo(2, 0x07, 's',
                value => BitConverter.GetBytes((short)value),
                raw => BitConverter.ToInt16(raw),
                value => value.ToString() + 's',
                raw => short.Parse(raw[..^1]));
        }

        public static LcsInfo IntInfo() {
            return new LcsInfo(4, 0x08, 'i',
                value => BitConverter.GetBytes((int)value),
                raw => BitConverter.ToInt32(raw),
                value => value.ToString() + 'i',
                raw => int.Parse(raw[..^1]));
        }

        public static LcsInfo LongInfo() {
            return new LcsInfo(8, 0x09, 'l',
                value => BitConverter.GetBytes((long)value),
                raw => BitConverter.ToInt64(raw),
                value => value.ToString() + 'l',
                raw => long.Parse(raw[..^1]));
        }

        public static LcsInfo FloatInfo() {
            return new LcsInfo(4, 0x0A, 'f',
                value => BitConverter.GetBytes((float)value),
                raw => BitConverter.ToSingle(raw),
                value => value.ToString() + 'f',
                raw => float.Parse(raw[..^1]));
        }

        public static LcsInfo DoubleInfo() {
            return new LcsInfo(8, 0x0B, 'd',
                value => BitConverter.GetBytes((double)value),
                raw => BitConverter.ToDouble(raw),
                value => value.ToString() + 'd',
                raw => double.Parse(raw[..^1]));
        }

        public static LcsInfo CharInfo() {
            return new LcsInfo(2, 0x0C, '-',
                value => BitConverter.GetBytes((char)value),
                raw => BitConverter.ToChar(raw),
                value =>
                {
                    var cast = (char)value;
                    return !SpecialChars.ContainsKey(cast) ? $"{value}-" : $@"\{SpecialChars[cast]}-";
                },
                raw => !raw.StartsWith(@"\") ? raw[0] : SpecialCodes[raw[1]]);
        }

        public static LcsInfo StringInfo() {
            return new LcsInfo(-1, 0x0D, '+',
                value => Encoding.UTF8.GetBytes((string)value),
                raw => Encoding.UTF8.GetString(raw),
                value => SpecialChars.Keys.Aggregate(((string)value).Replace(@"\", @"\\"),
                    (current, to) => current.Replace($"{to}", $@"\{SpecialChars[to]}")) + '+',
                raw => SpecialChars.Keys.Aggregate(raw[..^1].Replace(@"\\", @"\\ "),
                    (current, to) => current.Replace($@"\{SpecialChars[to]}", $"{to}")).Replace(@"\\ ", @"\"));
        }

        public static LcsInfo ArrayInfo() {
            return new LcsInfo(-1, 0x10, ';',
                value =>
                {
                    var cast = (object[])value;
                    var result = new List<byte>();
                    foreach (var item in cast)
                    {
                        result.AddRange(Binify(item));
                    }
                    return result.ToArray();
                },
                raw =>
                {
                    var result = new List<object>();
                    var cursor = 0;
                    while (cursor < raw.Length)
                    {
                        result.Add(Parse(raw, ref cursor));
                    }
                    return result.ToArray();
                },
                value => string.Join(',', ((object[])value).Select(Stringify)) + ';',
                raw => raw.Length > 1 ? raw[..^1].Split(',').Select(Parse).ToArray() : Array.Empty<object>());
        }

        // Utility functions
        public static IEnumerable<byte> WriteLengthBytes(int length)
        {
            var result = new List<byte>();
            while (length > 127)
            {
                result.Add((byte)(length | 0x80));
                length >>= 7;
            }
            result.Add((byte)length);
            return result;
        }

        public static int ReadLengthBytes(byte[] raw, ref int cursor)
        {
            var result = 0;
            var power = 1;
            while ((raw[cursor] & 0x80) == 0x80)
            {
                result += (raw[cursor++] & 0x7F) * power; 
                power <<= 7;
            }
            return result + raw[cursor++] * power;
        }
    }
}
