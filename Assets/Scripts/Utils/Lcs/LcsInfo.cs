using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Brutalsky.Logic;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Path;
using Utils.Player;

namespace Utils.Lcs
{
    public struct LcsInfo
    {
        public const string PropertySeparator = " / ";
        public const string FieldSeparator = " ";
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
            BoolInfo(), ByteInfo(), UShortInfo(), UIntInfo(), ULongInfo(), SByteInfo(), ShortInfo(), IntInfo(),
            LongInfo(), FloatInfo(), DoubleInfo(), DecimalInfo(), CharInfo(), StringInfo(), Int2Info(), Int3Info(),
            Float2Info(), Float3Info(), RectInfo(), ColorInfo(), DirectionInfo(), PlayerTypeInfo(), PathTypeInfo(),
            JointTypeInfo(), PathInfo(), PortInfo()
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

        public static byte[] Binify(object value)
        {
            var typeInfo = GetTypeInfo(value);
            var typeBytes = new[] { typeInfo.ByteTag };
            var valueBytes = typeInfo._toBin(value);
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

        public static object Parse(byte[] raw, ref int cursor)
        {
            var typeInfo = ByteTagTable[raw[cursor++]];
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
                decimal => DecimalInfo(),
                char => CharInfo(),
                string => StringInfo(),
                Vector2Int => Int2Info(),
                Vector3Int => Int3Info(),
                Vector2 => Float2Info(),
                Vector3 => Float3Info(),
                Rect => RectInfo(),
                Color => ColorInfo(),
                Direction => DirectionInfo(),
                PlayerType => PlayerTypeInfo(),
                PathType => PathTypeInfo(),
                JointType => JointTypeInfo(),
                PathString => PathInfo(),
                BsPort => PortInfo(),
                _ => throw Errors.InvalidItem("lcs type", value)
            };
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

        public static LcsInfo DecimalInfo() {
            return new LcsInfo(16, @"([\d\.]+|NaN|Infinity|-Infinity)m", 0x0C, 0m,
                _ => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
                _ => throw new NotImplementedException());
        }

        public static LcsInfo CharInfo() {
            return new LcsInfo(2, @"'(.|\\.)'", 0x0D, ' ',
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
            return new LcsInfo(-1, @"\""\S*\""", 0x0E, "",
                value => Encoding.UTF8.GetBytes((string)value),
                raw => Encoding.UTF8.GetString(raw),
                value => '"' + SpecialChars.Keys.Aggregate(((string)value).Replace(@"\", @"\\"),
                    (current, to) => current.Replace($"{to}", $@"\{SpecialChars[to]}")) + '"',
                raw => SpecialChars.Keys.Aggregate(raw[1..^1].Replace(@"\\", @"\\ "),
                    (current, to) => current.Replace($@"\{SpecialChars[to]}", $"{to}"))
                    .Replace(@"\\ ", @"\"));
        }

        // Compound types
        public static LcsInfo Int2Info() {
            return new LcsInfo(8, @"\(\d+ \d+\)i", 0x10, Vector2Int.zero,
                value =>
                {
                    var cast = (Vector2Int)value;
                    return BitConverter.GetBytes(cast.x).Concat(BitConverter.GetBytes(cast.y)).ToArray();
                },
                raw => new Vector2Int(BitConverter.ToInt32(raw[..4]), BitConverter.ToInt32(raw[4..8])),
                value =>
                {
                    var cast = (Vector2Int)value;
                    return '(' + ConcatFields(cast.x.ToString(), cast.y.ToString()) + ")i";
                },
                raw =>
                {
                    var parts = SplitFields(raw[1..^2]);
                    return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
                });
        }

        public static LcsInfo Int3Info() {
            return new LcsInfo(12, @"\(\d+ \d+\ \d+\)i", 0x11, Vector3Int.zero,
                value =>
                {
                    var cast = (Vector3Int)value;
                    return BitConverter.GetBytes(cast.x).Concat(BitConverter.GetBytes(cast.y)).ToArray();
                },
                raw => new Vector3Int(BitConverter.ToInt32(raw[..4]),
                    BitConverter.ToInt32(raw[4..8]), BitConverter.ToInt32(raw[8..12])),
                value =>
                {
                    var cast = (Vector3Int)value;
                    return '(' + ConcatFields(cast.x.ToString(), cast.y.ToString(), cast.z.ToString()) + ")i";
                },
                raw =>
                {
                    var parts = SplitFields(raw[1..^2]);
                    return new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                });
        }

        public static LcsInfo Float2Info() {
            return new LcsInfo(8, @"\([\d\.]+ [\d\.]+\)f", 0x12, Vector2.zero,
                value =>
                {
                    var cast = (Vector2)value;
                    return BitConverter.GetBytes(cast.x).Concat(BitConverter.GetBytes(cast.y)).ToArray();
                },
                raw => new Vector2(BitConverter.ToSingle(raw[..4]), BitConverter.ToSingle(raw[4..8])),
                value =>
                {
                    var cast = (Vector2)value;
                    return '(' + ConcatFields(cast.x.ToString(), cast.y.ToString()) + ")f";
                },
                raw =>
                {
                    var parts = SplitFields(raw[1..^2]);
                    return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
                });
        }

        public static LcsInfo Float3Info() {
            return new LcsInfo(12, @"\([\d\.]+ [\d\.]+ [\d\.]+\)f", 0x13, Vector3.zero,
                value =>
                {
                    var cast = (Vector3)value;
                    return BitConverter.GetBytes(cast.x).Concat(BitConverter.GetBytes(cast.y))
                        .Concat(BitConverter.GetBytes(cast.z)).ToArray();
                },
                raw => new Vector3(BitConverter.ToSingle(raw[..4]),
                    BitConverter.ToSingle(raw[4..8]), BitConverter.ToSingle(raw[8..12])),
                value =>
                {
                    var cast = (Vector3)value;
                    return '(' + ConcatFields(cast.x.ToString(), cast.y.ToString(), cast.z.ToString()) + ")f";
                },
                raw =>
                {
                    var parts = SplitFields(raw[1..^2]);
                    return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                });
        }

        public static LcsInfo RectInfo() {
            return new LcsInfo(16, @"\([\d\.]+ [\d\.]+ [\d\.]+ [\d\.]+\)", 0x14, Rect.zero,
                value =>
                {
                    var cast = (Rect)value;
                    return BitConverter.GetBytes(cast.x).Concat(BitConverter.GetBytes(cast.y))
                        .Concat(BitConverter.GetBytes(cast.width)).Concat(BitConverter.GetBytes(cast.height))
                        .ToArray();
                },
                raw => new Rect(BitConverter.ToSingle(raw[..4]), BitConverter.ToSingle(raw[4..8]),
                    BitConverter.ToSingle(raw[8..12]), BitConverter.ToSingle(raw[12..16])),
                value =>
                {
                    var cast = (Rect)value;
                    return '(' + ConcatFields(cast.x.ToString(), cast.y.ToString(),
                        cast.width.ToString(), cast.height.ToString()) + ')';
                },
                raw =>
                {
                    var parts = SplitFields(raw[1..^1]);
                    return new Rect(float.Parse(parts[0]), float.Parse(parts[1]),
                        float.Parse(parts[2]), float.Parse(parts[3]));
                });
        }

        public static LcsInfo ColorInfo() {
            return new LcsInfo(4, "[0-9A-F]{8}", 0x15, Color.black,
                value =>
                {
                    var cast = (Color)value;
                    return new[] { Float01ToByte(cast.r), Float01ToByte(cast.g),
                        Float01ToByte(cast.b), Float01ToByte(cast.a) };
                },
                raw => new Color(ByteToFloat01(raw[0]), ByteToFloat01(raw[1]),
                    ByteToFloat01(raw[2]), ByteToFloat01(raw[3])),
                value =>
                {
                    var cast = (Color)value;
                    return Float01ToHex(cast.r) + Float01ToHex(cast.g) +
                           Float01ToHex(cast.b) + Float01ToHex(cast.a);
                },
                raw => new Color(HexToFloat01(raw[..2]), HexToFloat01(raw[2..4]),
                    HexToFloat01(raw[4..6]), HexToFloat01(raw[6..8])));
        }

        // Enum types
        public static LcsInfo DirectionInfo() {
            return new LcsInfo(1, "None|Down|Up|Left|Right", 0x20, Direction.None,
                value => new[] { (byte)(Direction)value },
                raw => (Direction)raw[0],
                value => value.ToString(),
                raw => (Direction)Enum.Parse(typeof(Direction), raw));
        }

        public static LcsInfo PlayerTypeInfo() {
            return new LcsInfo(1, "Dummy|Main|Local1|Local2|Bot", 0x21, PlayerType.Dummy,
                value => new[] { (byte)(PlayerType)value },
                raw => (PlayerType)raw[0],
                value => value.ToString(),
                raw => (PlayerType)Enum.Parse(typeof(PlayerType), raw));
        }

        public static LcsInfo PathTypeInfo() {
            return new LcsInfo(1, "Vector|Polygon|Square|Rectangle|Circle|Ellipse|Ngon|Star", 0x22, PathType.Vector,
                value => new[] { (byte)(PathType)value },
                raw => (PathType)raw[0],
                value => value.ToString(),
                raw => (PathType)Enum.Parse(typeof(PathType), raw));
        }

        public static LcsInfo JointTypeInfo() {
            return new LcsInfo(1, "Fixed|Distance|Spring|Hinge|Slider|Wheel", 0x23, JointType.Fixed,
                value => new[] { (byte)(JointType)value },
                raw => (JointType)raw[0],
                value => value.ToString(),
                raw => (JointType)Enum.Parse(typeof(JointType), raw));
        }

        // Brutalsky types
        public static LcsInfo PathInfo() {
            return new LcsInfo(-1, @"(Vector|Polygon|Square|Rectangle|Circle|Ellipse|Ngon|Star)( [\d\.]+)+", 0x31,
                PathString.Ngon(3, 1f),
                value =>
                {
                    var cast = (PathString)value;
                    var result = new byte[cast.Args.Length * 4 + 1];
                    result[0] = (byte)cast.Type;
                    for (var i = 0; i < cast.Args.Length; i++)
                    {
                        var argBytes = BitConverter.GetBytes(cast.Args[i]);
                        var bitIndex = i * 4 + 1;
                        for (var j = 0; j < 4; j++)
                        {
                            result[bitIndex + j] = argBytes[j];
                        }
                    }
                    return result;
                },
                raw =>
                {
                    var type = (PathType)raw[0];
                    var parts = new float[(raw.Length - 1) / 4];
                    for (var i = 0; i < parts.Length; i++)
                    {
                        var bitIndex = i * 4 + 1;
                        parts[i] = BitConverter.ToSingle(raw[bitIndex..(bitIndex + 4)]);
                    }
                    return type switch
                    {
                        PathType.Vector => PathString.Vector(parts),
                        PathType.Polygon => PathString.Polygon(parts),
                        PathType.Square => PathString.Square(parts[0]),
                        PathType.Rectangle => PathString.Rectangle(parts[0], parts[1]),
                        PathType.Circle => PathString.Circle(parts[0]),
                        PathType.Ellipse => PathString.Ellipse(parts[0], parts[1]),
                        PathType.Ngon => PathString.Ngon(Mathf.RoundToInt(parts[0]), parts[1]),
                        PathType.Star => PathString.Star(Mathf.RoundToInt(parts[0]), parts[1], parts[2]),
                        _ => throw Errors.InvalidItem("path type", type)
                    };
                },
                value => ((PathString)value).Args.Aggregate(value.ToString(),
                    (current, arg) => current + FieldSeparator + arg),
                raw =>
                {
                    var parts = raw.Split(FieldSeparator);
                    var type = (PathType)Enum.Parse(typeof(PathType), parts[0]);
                    var args = parts[1..].Select(part => float.Parse(part)).ToArray();
                    return type switch
                    {
                        PathType.Vector => PathString.Vector(args),
                        PathType.Polygon => PathString.Polygon(args),
                        PathType.Square => PathString.Square(args[0]),
                        PathType.Rectangle => PathString.Rectangle(args[0], args[1]),
                        PathType.Circle => PathString.Circle(args[0]),
                        PathType.Ellipse => PathString.Ellipse(args[0], args[1]),
                        PathType.Ngon => PathString.Ngon((int)args[0], args[1]),
                        PathType.Star => PathString.Star((int)args[0], args[1], args[2]),
                        _ => throw Errors.InvalidItem("path type", type)
                    };
                });
        }

        public static LcsInfo PortInfo() {
            return new LcsInfo(3, "[0-9A-F]{6}", 0x34, new BsPort(0, 0),
                value =>
                {
                    var cast = (BsPort)value;
                    return BitConverter.GetBytes(cast.NodeId).Append(cast.PortId).ToArray();
                },
                raw => new BsPort(BitConverter.ToUInt16(raw[..2]), raw[2]),
                value =>
                {
                    var cast = (BsPort)value;
                    return ConcatFields(IntToHex(cast.NodeId, 4), IntToHex(cast.PortId, 2));
                },
                raw =>
                {
                    return new BsPort((ushort)HexToInt(raw[..4]), (byte)HexToInt(raw[4..6]));
                });
        }

        // Utility functions
        public static byte Float01ToByte(float value)
        {
            return (byte)(value * 255);
        }

        public static float ByteToFloat01(byte value)
        {
            return value / 255f;
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

        public static string IntToHex(int value, int digits)
        {
            return value.ToString($"X{digits}");
        }

        public static int HexToInt(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        public static string Float01ToHex(float value)
        {
            return IntToHex((int)(value * 255), 2);
        }

        public static float HexToFloat01(string hex)
        {
            return HexToInt(hex) / 255f;
        }

        public static string ConcatProps(params string[] items)
        {
            return string.Join(PropertySeparator, items);
        }

        public static string[] SplitProps(string items)
        {
            return items.Split(PropertySeparator);
        }

        public static string ConcatFields(params string[] items)
        {
            return string.Join(FieldSeparator, items);
        }

        public static string[] SplitFields(string items)
        {
            return items.Split(FieldSeparator);
        }
    }
}
