using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Brutalsky.Scripts.Utils;

namespace Brutalsky.Scripts.Lcs;

public struct LcsInfo
{
    public const char PropertySeparator = ' ';
    public static readonly Dictionary<char, char> SpecialChars = new()
    {
        {'\'', 'q'},
        {'"', 'u'},
        {'/', 'd'},
        {' ', 's'},
        {'\t', 't'},
        {'\n', 'n'}
    };
    public static readonly Dictionary<char, char> SpecialCodes =
        SpecialChars.ToDictionary(kv => kv.Value, kv => kv.Key);

    public static readonly List<LcsInfo> TypeInfoList = new()
    {
        BoolInfo(), ByteInfo(), UShortInfo(), UIntInfo(), ULongInfo(), SByteInfo(), ShortInfo(), IntInfo(), LongInfo(),
        FloatInfo(), DoubleInfo(), CharInfo(), StringInfo(),
    };
    public static readonly Dictionary<byte, LcsInfo> ByteTagTable =
        TypeInfoList.ToDictionary(type => type.ByteTag, type => type);

    public int Size { get; }
    public byte ByteTag { get; }
    public Regex StringPattern { get; }

    private Func<object, byte[]> _toBin;
    private Func<byte[], object> _fromBin;
    private Func<object, string> _toStr;
    private Func<string, object> _fromStr;

    public LcsInfo(int size, byte byteTag, string stringPattern,
        Func<object, byte[]> toBin, Func<byte[], object> fromBin,
        Func<object, string> toStr, Func<string, object> fromStr)
    {
        Size = size;
        ByteTag = byteTag;
        StringPattern = new Regex('^' + stringPattern + '$');
        _toBin = toBin;
        _fromBin = fromBin;
        _toStr = toStr;
        _fromStr = fromStr;
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
            _ => throw Errors.InvalidItem("lcs type", value)
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
        return TypeInfoList.First(type => type.StringPattern.IsMatch(raw))._fromStr(raw);
    }

    // Primitive types
    public static LcsInfo BoolInfo() {
        return new LcsInfo(1, 0x01, "True|False",
            value => BitConverter.GetBytes((bool)value),
            raw => BitConverter.ToBoolean(raw),
            value => value.ToString(),
            raw => bool.Parse(raw));
    }

    public static LcsInfo ByteInfo() {
        return new LcsInfo(1, 0x02, @"\d+B",
            value => new[] { (byte)value },
            raw => raw[0],
            value => value.ToString() + 'B',
            raw => byte.Parse(raw[..^1]));
    }

    public static LcsInfo UShortInfo() {
        return new LcsInfo(2, 0x03, @"\d+S",
            value => BitConverter.GetBytes((ushort)value),
            raw => BitConverter.ToUInt16(raw),
            value => value.ToString() + 'S',
            raw => ushort.Parse(raw[..^1]));
    }

    public static LcsInfo UIntInfo() {
        return new LcsInfo(4, 0x04, @"\d+I",
            value => BitConverter.GetBytes((uint)value),
            raw => BitConverter.ToUInt32(raw),
            value => value.ToString() + 'I',
            raw => uint.Parse(raw[..^1]));
    }

    public static LcsInfo ULongInfo() {
        return new LcsInfo(8, 0x05, @"\d+L",
            value => BitConverter.GetBytes((ulong)value),
            raw => BitConverter.ToUInt64(raw),
            value => value.ToString() + 'L',
            raw => ulong.Parse(raw[..^1]));
    }

    public static LcsInfo SByteInfo() {
        return new LcsInfo(1, 0x06, @"\d+b",
            value => new[] { (byte)(sbyte)value },
            raw => (sbyte)raw[0],
            value => value.ToString() + 'b',
            raw => sbyte.Parse(raw[..^1]));
    }

    public static LcsInfo ShortInfo() {
        return new LcsInfo(2, 0x07, @"\d+s",
            value => BitConverter.GetBytes((short)value),
            raw => BitConverter.ToInt16(raw),
            value => value.ToString() + 's',
            raw => short.Parse(raw[..^1]));
    }

    public static LcsInfo IntInfo() {
        return new LcsInfo(4, 0x08, @"\d+i",
            value => BitConverter.GetBytes((int)value),
            raw => BitConverter.ToInt32(raw),
            value => value.ToString() + 'i',
            raw => int.Parse(raw[..^1]));
    }

    public static LcsInfo LongInfo() {
        return new LcsInfo(8, 0x09, @"\d+l",
            value => BitConverter.GetBytes((long)value),
            raw => BitConverter.ToInt64(raw),
            value => value.ToString() + 'l',
            raw => long.Parse(raw[..^1]));
    }

    public static LcsInfo FloatInfo() {
        return new LcsInfo(4, 0x0A, @"([\d\.]+|NaN|Infinity|-Infinity)f",
            value => BitConverter.GetBytes((float)value),
            raw => BitConverter.ToSingle(raw),
            value => value.ToString() + 'f',
            raw => float.Parse(raw[..^1]));
    }

    public static LcsInfo DoubleInfo() {
        return new LcsInfo(8, 0x0B, @"([\d\.]+|NaN|Infinity|-Infinity)d",
            value => BitConverter.GetBytes((double)value),
            raw => BitConverter.ToDouble(raw),
            value => value.ToString() + 'd',
            raw => double.Parse(raw[..^1]));
    }

    public static LcsInfo CharInfo() {
        return new LcsInfo(2, 0x0C, @"'(.|\\.)'",
            value => BitConverter.GetBytes((char)value),
            raw => BitConverter.ToChar(raw),
            value =>
            {
                var cast = (char)value;
                return !SpecialChars.ContainsKey(cast) ? $"'{value}'" : $@"'\{SpecialChars[cast]}'";
            },
            raw => !raw.StartsWith(@"\") ? raw[0] : SpecialCodes[raw[1]]);
    }

    public static LcsInfo StringInfo() {
        return new LcsInfo(-1, 0x0D, @"\""\S*\""",
            value => Encoding.UTF8.GetBytes((string)value),
            raw => Encoding.UTF8.GetString(raw),
            value => '"' + SpecialChars.Keys.Aggregate(((string)value).Replace(@"\", @"\\"),
                (current, to) => current.Replace($"{to}", $@"\{SpecialChars[to]}")) + '"',
            raw => SpecialChars.Keys.Aggregate(raw[1..^1].Replace(@"\\", @"\\ "),
                    (current, to) => current.Replace($@"\{SpecialChars[to]}", $"{to}"))
                .Replace(@"\\ ", @"\"));
    }

    // Utility functions
    public static IEnumerable<byte> WriteLengthBytes(int length)
    {
        var result = new List<byte>();
        while (length > 127)
        {
            result.Add((byte)(length | 0x80));
            length /= 128;
        }
        result.Add((byte)length);
        return result;
    }

    public static int ReadLengthBytes(byte[] raw, ref int cursor)
    {
        var result = 0;
        var power = 1;
        while ((raw[cursor] & 0x80) > 0)
        {
            result += (raw[cursor++] & 0x7F) * power; 
            power *= 128;
        }
        return result + raw[cursor++] * power;
    }

    public static string ConcatProps(params string[] items)
    {
        return string.Join(PropertySeparator, items);
    }

    public static string[] SplitProps(string items)
    {
        return items.Split(PropertySeparator);
    }
}
