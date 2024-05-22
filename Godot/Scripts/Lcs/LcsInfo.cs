using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Brutalsky.Scripts.Utils;

namespace Brutalsky.Scripts.Lcs;

public struct LcsInfo
{
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
        FloatInfo(), DoubleInfo(), CharInfo(), StringInfo()
    };
    public static readonly Dictionary<byte, LcsInfo> ByteTagTable =
        TypeInfoList.ToDictionary(type => type.ByteTag, type => type);

    public int Size { get; }
    public Regex StringPattern { get; }
    public byte ByteTag { get; }
    public object DefaultValue { get; }

    private Func<object, byte[]> _toBin;
    private Func<byte[], object> _fromBin;
    private Func<object, string> _toStr;
    private Func<string, object> _fromStr;

    public LcsInfo(int size, string stringPattern, byte byteTag, object defaultValue,
        Func<object, byte[]> toBin, Func<byte[], object> fromBin,
        Func<object, string> toStr, Func<string, object> fromStr)
    {
        Size = size;
        StringPattern = new Regex('^' + stringPattern + '$');
        ByteTag = byteTag;
        DefaultValue = defaultValue;
        _toBin = toBin;
        _fromBin = fromBin;
        _toStr = toStr;
        _fromStr = fromStr;
    }

    public static LcsDocument Serialize(ILcsDocument source)
    {
        return source._ToLcs();
    }

    public static T Parse<T>(LcsDocument raw) where T : ILcsDocument, new()
    {
        var result = new T();
        result._FromLcs(raw);
        return result;
    }

    public static LcsLine Serialize(ILcsLine source)
    {
        return source._ToLcs();
    }

    public static T Parse<T>(LcsLine raw) where T : ILcsLine, new()
    {
        var result = new T();
        result._FromLcs(raw);
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
            _ => throw Errors.InvalidItem("lcs type", value)
        };
    }

    public static byte[] Binify(object value)
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
        return new LcsInfo(1, "True|False", 0x01, false,
            value => BitConverter.GetBytes((bool)value),
            raw => BitConverter.ToBoolean(raw),
            value => value.ToString(),
            raw => bool.Parse(raw));
    }

    public static LcsInfo ByteInfo() {
        return new LcsInfo(1, @"\d+B", 0x02, (byte)0,
            value => new[] { (byte)value },
            raw => raw[0],
            value => value.ToString() + 'B',
            raw => byte.Parse(raw[..^1]));
    }

    public static LcsInfo UShortInfo() {
        return new LcsInfo(2, @"\d+S", 0x03, (ushort)0,
            value => BitConverter.GetBytes((ushort)value),
            raw => BitConverter.ToUInt16(raw),
            value => value.ToString() + 'S',
            raw => ushort.Parse(raw[..^1]));
    }

    public static LcsInfo UIntInfo() {
        return new LcsInfo(4, @"\d+I", 0x04, 0u,
            value => BitConverter.GetBytes((uint)value),
            raw => BitConverter.ToUInt32(raw),
            value => value.ToString() + 'I',
            raw => uint.Parse(raw[..^1]));
    }

    public static LcsInfo ULongInfo() {
        return new LcsInfo(8, @"\d+L", 0x05, 0uL,
            value => BitConverter.GetBytes((ulong)value),
            raw => BitConverter.ToUInt64(raw),
            value => value.ToString() + 'L',
            raw => ulong.Parse(raw[..^1]));
    }

    public static LcsInfo SByteInfo() {
        return new LcsInfo(1, @"\d+b", 0x06, (sbyte)0,
            value => new[] { (byte)(sbyte)value },
            raw => (sbyte)raw[0],
            value => value.ToString() + 'b',
            raw => sbyte.Parse(raw[..^1]));
    }

    public static LcsInfo ShortInfo() {
        return new LcsInfo(2, @"\d+s", 0x07, (short)0,
            value => BitConverter.GetBytes((short)value),
            raw => BitConverter.ToInt16(raw),
            value => value.ToString() + 's',
            raw => short.Parse(raw[..^1]));
    }

    public static LcsInfo IntInfo() {
        return new LcsInfo(4, @"\d+i", 0x08, 0,
            value => BitConverter.GetBytes((int)value),
            raw => BitConverter.ToInt32(raw),
            value => value.ToString() + 'i',
            raw => int.Parse(raw[..^1]));
    }

    public static LcsInfo LongInfo() {
        return new LcsInfo(8, @"\d+l", 0x09, 0L,
            value => BitConverter.GetBytes((long)value),
            raw => BitConverter.ToInt64(raw),
            value => value.ToString() + 'l',
            raw => long.Parse(raw[..^1]));
    }

    public static LcsInfo FloatInfo() {
        return new LcsInfo(4, @"([\d\.]+|NaN|Infinity|-Infinity)f", 0x0A, 0f,
            value => BitConverter.GetBytes((float)value),
            raw => BitConverter.ToSingle(raw),
            value => value.ToString() + 'f',
            raw => float.Parse(raw[..^1]));
    }

    public static LcsInfo DoubleInfo() {
        return new LcsInfo(8, @"([\d\.]+|NaN|Infinity|-Infinity)d", 0x0B, 0d,
            value => BitConverter.GetBytes((double)value),
            raw => BitConverter.ToDouble(raw),
            value => value.ToString() + 'd',
            raw => double.Parse(raw[..^1]));
    }

    public static LcsInfo CharInfo() {
        return new LcsInfo(2, @"'(.|\\.)'", 0x0C, ' ',
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
        return new LcsInfo(-1, @"\""\S*\""", 0x0D, "",
            value => Encoding.UTF8.GetBytes((string)value),
            raw => Encoding.UTF8.GetString(raw),
            value => '"' + SpecialChars.Keys.Aggregate(((string)value).Replace(@"\", @"\\"),
                (current, to) => current.Replace($"{to}", $@"\{SpecialChars[to]}")) + '"',
            raw => SpecialChars.Keys.Aggregate(raw[1..^1].Replace(@"\\", @"\\ "),
                    (current, to) => current.Replace($@"\{SpecialChars[to]}", $"{to}"))
                .Replace(@"\\ ", @"\"));
    }

    // Utility functions
    public static byte[] WriteLengthBytes(int length)
    {
        var result = new List<byte>();
        while (length > 127)
        {
            result.Add((byte)(length | 0x80));
            length /= 128;
        }
        result.Add((byte)length);
        return result.ToArray();
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

    public static byte[] GzipCompress(byte[] source)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gzipStream.Write(source, 0, source.Length);
        }
        return memoryStream.ToArray();
    }

    public static byte[] GzipDecompress(byte[] source)
    {
        using var memoryStream = new MemoryStream(source);
        using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        gzipStream.CopyTo(decompressedStream);
        return decompressedStream.ToArray();
    }

    public static string ConcatProps(params string[] items)
    {
        return string.Join(" / ", items);
    }

    public static string[] SplitProps(string items)
    {
        return items.Split(" / ");
    }
}
