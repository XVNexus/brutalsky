using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Brutalsky.Logic;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Object;
using Utils.Player;
using Utils.Pool;
using Utils.Shape;

namespace Utils.Lcs
{
    public struct LcsInfo
    {
        public const string LineSeparator = "\n";
        public const string PrefixSeparator = " ";
        public const string PropertySeparator = " / ";
        public const string TypeSeparator = ": ";
        public const string FieldSeparator = " ";
        public static readonly Dictionary<char, char> SpecialChars = new()
        {
            {'/', 'p'},
            {':', 'y'},
            {' ', 's'},
            {'\t', 't'},
            {'\n', 'n'}
        };
        public static readonly Dictionary<char, char> SpecialCodes =
            SpecialChars.ToDictionary(kv => kv.Value, kv => kv.Key);

        public static readonly Dictionary<LcsType, LcsInfo> TypeTable = new()
        {
            { LcsType.Bool, BoolInfo() },
            { LcsType.Byte, ByteInfo() },
            { LcsType.UShort, UShortInfo() },
            { LcsType.UInt, UIntInfo() },
            { LcsType.ULong, ULongInfo() },
            { LcsType.SByte, SByteInfo() },
            { LcsType.Short, ShortInfo() },
            { LcsType.Int, IntInfo() },
            { LcsType.Long, LongInfo() },
            { LcsType.Float, FloatInfo() },
            { LcsType.Double, DoubleInfo() },
            { LcsType.Decimal, DecimalInfo() },
            { LcsType.Char, CharInfo() },
            { LcsType.String, StringInfo() },
            { LcsType.Int2, Int2Info() },
            { LcsType.Int3, Int3Info() },
            { LcsType.Float2, Float2Info() },
            { LcsType.Float3, Float3Info() },
            { LcsType.Rect, RectInfo() },
            { LcsType.Color, ColorInfo() },
            { LcsType.PlayerType, PlayerTypeInfo() },
            { LcsType.Direction, DirectionInfo() },
            { LcsType.Layer, LayerInfo() },
            { LcsType.FormType, FormTypeInfo() },
            { LcsType.JointType, JointTypeInfo() },
            { LcsType.Transform, TransformInfo() },
            { LcsType.Form, FormInfo() },
            { LcsType.Material, MaterialInfo() },
            { LcsType.Chemical, ChemicalInfo() },
            { LcsType.Port, PortInfo() }
        };
        public static readonly Dictionary<string, LcsType> StringTagTable =
            TypeTable.ToDictionary(kv => kv.Value.StringTag, kv => kv.Key);
        public static readonly Dictionary<byte, LcsType> ByteTagTable =
            TypeTable.ToDictionary(kv => kv.Value.ByteTag, kv => kv.Key);

        public int Size { get; }
        public string StringTag { get; }
        public byte ByteTag { get; }
        public Func<object, byte[]> ToBin { get; }
        public Func<byte[], object> FromBin { get; }
        public Func<object, string> ToStr { get; }
        public Func<string, object> FromStr { get; }

        public LcsInfo(int size, string stringTag, byte byteTag,
            Func<object, byte[]> toBin, Func<byte[], object> fromBin,
            Func<object, string> toStr, Func<string, object> fromStr)
        {
            Size = size;
            StringTag = stringTag;
            ByteTag = byteTag;
            ToBin = toBin;
            FromBin = fromBin;
            ToStr = toStr;
            FromStr = fromStr;
        }

        // Primitive types
        public static LcsInfo BoolInfo() {
            return new LcsInfo(1, "bol", 0x01,
                value => BitConverter.GetBytes((bool)value),
                raw => BitConverter.ToBoolean(raw),
                value => value.ToString(),
                raw => bool.Parse(raw));
        }

        public static LcsInfo ByteInfo() {
            return new LcsInfo(1, "uby", 0x02,
                value => new[] { (byte)value },
                raw => raw[0],
                value => value.ToString(),
                raw => byte.Parse(raw));
        }

        public static LcsInfo UShortInfo() {
            return new LcsInfo(2, "usr", 0x03,
                value => BitConverter.GetBytes((ushort)value),
                raw => BitConverter.ToUInt16(raw),
                value => value.ToString(),
                raw => ushort.Parse(raw));
        }

        public static LcsInfo UIntInfo() {
            return new LcsInfo(4, "uin", 0x04,
                value => BitConverter.GetBytes((uint)value),
                raw => BitConverter.ToUInt32(raw),
                value => value.ToString(),
                raw => uint.Parse(raw));
        }

        public static LcsInfo ULongInfo() {
            return new LcsInfo(8, "ulo", 0x05,
                value => BitConverter.GetBytes((ulong)value),
                raw => BitConverter.ToUInt64(raw),
                value => value.ToString(),
                raw => ulong.Parse(raw));
        }

        public static LcsInfo SByteInfo() {
            return new LcsInfo(1, "byt", 0x06,
                value => new[] { (byte)value },
                raw => (sbyte)raw[0],
                value => value.ToString(),
                raw => sbyte.Parse(raw));
        }

        public static LcsInfo ShortInfo() {
            return new LcsInfo(2, "srt", 0x07,
                value => BitConverter.GetBytes((short)value),
                raw => BitConverter.ToInt16(raw),
                value => value.ToString(),
                raw => short.Parse(raw));
        }

        public static LcsInfo IntInfo() {
            return new LcsInfo(4, "int", 0x08,
                value => BitConverter.GetBytes((int)value),
                raw => BitConverter.ToInt32(raw),
                value => value.ToString(),
                raw => int.Parse(raw));
        }

        public static LcsInfo LongInfo() {
            return new LcsInfo(8, "lng", 0x09,
                value => BitConverter.GetBytes((long)value),
                raw => BitConverter.ToInt64(raw),
                value => value.ToString(),
                raw => long.Parse(raw));
        }

        public static LcsInfo FloatInfo() {
            return new LcsInfo(4, "flt", 0x0A,
                value => BitConverter.GetBytes((float)value),
                raw => BitConverter.ToSingle(raw),
                value => value.ToString(),
                raw => float.Parse(raw));
        }

        public static LcsInfo DoubleInfo() {
            return new LcsInfo(8, "dbl", 0x0B,
                value => BitConverter.GetBytes((double)value),
                raw => BitConverter.ToDouble(raw),
                value => value.ToString(),
                raw => double.Parse(raw));
        }

        public static LcsInfo DecimalInfo() {
            return new LcsInfo(16, "dec", 0x0C,
                value => throw new NotImplementedException(),
                raw => throw new NotImplementedException(),
                value => throw new NotImplementedException(),
                raw => throw new NotImplementedException());
        }

        public static LcsInfo CharInfo() {
            return new LcsInfo(2, "chr", 0x0D,
                value => BitConverter.GetBytes((char)value),
                raw => BitConverter.ToChar(raw),
                value =>
                {
                    var cast = (char)value;
                    return !SpecialChars.ContainsKey(cast) ? value.ToString() : $@"\{SpecialChars[cast]}";
                },
                raw => !raw.StartsWith(@"\") ? raw[0] : SpecialCodes[raw[1]]);
        }

        public static LcsInfo StringInfo() {
            return new LcsInfo(-1, "str", 0x0E,
                value => Encoding.UTF8.GetBytes((string)value),
                raw => Encoding.UTF8.GetString(raw),
                value => SpecialChars.Keys.Aggregate(((string)value).Replace(@"\", @"\\"),
                    (current, to) => current.Replace($"{to}", $@"\{SpecialChars[to]}")),
                raw => SpecialChars.Keys.Aggregate(raw.Replace(@"\\", @"\\ "),
                    (current, to) => current.Replace($@"\{SpecialChars[to]}", $"{to}"))
                    .Replace(@"\\ ", @"\"));
        }

        // Compound types
        public static LcsInfo Int2Info() {
            return new LcsInfo(8, "in2", 0x10,
                value =>
                {
                    var cast = (Vector2Int)value;
                    return BitConverter.GetBytes(cast.x).Concat(BitConverter.GetBytes(cast.y)).ToArray();
                },
                raw => new Vector2Int(BitConverter.ToInt32(raw[..4]), BitConverter.ToInt32(raw[4..8])),
                value =>
                {
                    var cast = (Vector2Int)value;
                    return ConcatFields(cast.x.ToString(), cast.y.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
                });
        }

        public static LcsInfo Int3Info() {
            return new LcsInfo(12, "in3", 0x11,
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
                    return ConcatFields(cast.x.ToString(), cast.y.ToString(), cast.z.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                });
        }

        public static LcsInfo Float2Info() {
            return new LcsInfo(8, "fl2", 0x12,
                value =>
                {
                    var cast = (Vector2)value;
                    return BitConverter.GetBytes(cast.x).Concat(BitConverter.GetBytes(cast.y)).ToArray();
                },
                raw => new Vector2(BitConverter.ToSingle(raw[..4]), BitConverter.ToSingle(raw[4..8])),
                value =>
                {
                    var cast = (Vector2)value;
                    return ConcatFields(cast.x.ToString(), cast.y.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
                });
        }

        public static LcsInfo Float3Info() {
            return new LcsInfo(12, "fl3", 0x13,
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
                    return ConcatFields(cast.x.ToString(), cast.y.ToString(), cast.z.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                });
        }

        public static LcsInfo RectInfo() {
            return new LcsInfo(16, "rec", 0x14,
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
                    return ConcatFields(cast.x.ToString(), cast.y.ToString(),
                        cast.width.ToString(), cast.height.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new Rect(float.Parse(parts[0]), float.Parse(parts[1]),
                        float.Parse(parts[2]), float.Parse(parts[3]));
                });
        }

        public static LcsInfo ColorInfo() {
            return new LcsInfo(4, "col", 0x15,
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
            return new LcsInfo(1, "dir", 0x20,
                value => new[] { (byte)(Direction)value },
                raw => (Direction)raw[0],
                value => value.ToString(),
                raw => (Direction)Enum.Parse(typeof(Direction), raw));
        }

        public static LcsInfo LayerInfo() {
            return new LcsInfo(1, "lay", 0x21,
                value => new[] { (byte)(ObjectLayer)value },
                raw => (ObjectLayer)raw[0],
                value => value.ToString(),
                raw => (ObjectLayer)Enum.Parse(typeof(ObjectLayer), raw));
        }

        public static LcsInfo PlayerTypeInfo() {
            return new LcsInfo(1, "plt", 0x22,
                value => new[] { (byte)(PlayerType)value },
                raw => (PlayerType)raw[0],
                value => value.ToString(),
                raw => (PlayerType)Enum.Parse(typeof(PlayerType), raw));
        }

        public static LcsInfo FormTypeInfo() {
            return new LcsInfo(1, "frt", 0x23,
                value => new[] { (byte)(FormType)value },
                raw => (FormType)raw[0],
                value => value.ToString(),
                raw => (FormType)Enum.Parse(typeof(FormType), raw));
        }

        public static LcsInfo JointTypeInfo() {
            return new LcsInfo(1, "jnt", 0x24,
                value => new[] { (byte)(JointType)value },
                raw => (JointType)raw[0],
                value => value.ToString(),
                raw => (JointType)Enum.Parse(typeof(JointType), raw));
        }

        // Brutalsky types
        public static LcsInfo TransformInfo() {
            return new LcsInfo(12, "trn", 0x30,
                value =>
                {
                    var cast = (ObjectTransform)value;
                    return BitConverter.GetBytes(cast.Position.x).Concat(BitConverter.GetBytes(cast.Position.y))
                        .Concat(BitConverter.GetBytes(cast.Rotation)).ToArray();
                },
                raw => new ObjectTransform(BitConverter.ToSingle(raw[..4]),
                    BitConverter.ToSingle(raw[4..8]), BitConverter.ToSingle(raw[8..12])),
                value =>
                {
                    var cast = (ObjectTransform)value;
                    return ConcatFields(cast.Position.x.ToString(), cast.Position.y.ToString(),
                        cast.Rotation.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new ObjectTransform(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                });
        }

        public static LcsInfo FormInfo() {
            return new LcsInfo(-1, "for", 0x31,
                value =>
                {
                    var cast = (Form)value;
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
                    var type = (FormType)raw[0];
                    var parts = new float[(raw.Length - 1) / 4];
                    for (var i = 0; i < parts.Length; i++)
                    {
                        var bitIndex = i * 4 + 1;
                        parts[i] = BitConverter.ToSingle(raw[bitIndex..(bitIndex + 4)]);
                    }
                    return type switch
                    {
                        FormType.Vector => Form.Vector(parts),
                        FormType.Polygon => Form.Polygon(parts),
                        FormType.Square => Form.Square(parts[0]),
                        FormType.Rectangle => Form.Rectangle(parts[0], parts[1]),
                        FormType.Circle => Form.Circle(parts[0]),
                        FormType.Ellipse => Form.Ellipse(parts[0], parts[1]),
                        FormType.Ngon => Form.Ngon(Mathf.RoundToInt(parts[0]), parts[1]),
                        FormType.Star => Form.Star(Mathf.RoundToInt(parts[0]), parts[1], parts[2]),
                        _ => Form.Invalid()
                    };
                },
                value => ((Form)value).Args.Aggregate(value.ToString(),
                    (current, arg) => current + FieldSeparator + arg),
                raw =>
                {
                    var parts = raw.Split(FieldSeparator);
                    var type = (FormType)Enum.Parse(typeof(FormType), parts[0]);
                    var args = parts[1..].Select(part => float.Parse(part)).ToArray();
                    return type switch
                    {
                        FormType.Vector => Form.Vector(args),
                        FormType.Polygon => Form.Polygon(args),
                        FormType.Square => Form.Square(args[0]),
                        FormType.Rectangle => Form.Rectangle(args[0], args[1]),
                        FormType.Circle => Form.Circle(args[0]),
                        FormType.Ellipse => Form.Ellipse(args[0], args[1]),
                        FormType.Ngon => Form.Ngon((int)args[0], args[1]),
                        FormType.Star => Form.Star((int)args[0], args[1], args[2]),
                        _ => Form.Invalid()
                    };
                });
        }

        public static LcsInfo MaterialInfo() {
            return new LcsInfo(20, "mat", 0x32,
                value =>
                {
                    var cast = (ShapeMaterial)value;
                    return BitConverter.GetBytes(cast.Friction).Concat(BitConverter.GetBytes(cast.Restitution))
                        .Concat(BitConverter.GetBytes(cast.Adhesion)).Concat(BitConverter.GetBytes(cast.Density))
                        .Concat(BitConverter.GetBytes(cast.Health)).ToArray();
                },
                raw => new ShapeMaterial(BitConverter.ToSingle(raw[..4]), BitConverter.ToSingle(raw[4..8]),
                    BitConverter.ToSingle(raw[8..12]), BitConverter.ToSingle(raw[12..16]),
                    BitConverter.ToSingle(raw[16..20])),
                value =>
                {
                    var cast = (ShapeMaterial)value;
                    return ConcatFields(cast.Friction.ToString(), cast.Restitution.ToString(),
                        cast.Adhesion.ToString(), cast.Density.ToString(), cast.Health.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new ShapeMaterial(float.Parse(parts[0]), float.Parse(parts[1]),
                        float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
                });
        }

        public static LcsInfo ChemicalInfo() {
            return new LcsInfo(12, "cem", 0x33,
                value =>
                {
                    var cast = (PoolChemical)value;
                    return BitConverter.GetBytes(cast.Buoyancy).Concat(BitConverter.GetBytes(cast.Viscosity))
                        .Concat(BitConverter.GetBytes(cast.Health)).ToArray();
                },
                raw => new PoolChemical(BitConverter.ToSingle(raw[..4]),
                    BitConverter.ToSingle(raw[4..8]), BitConverter.ToSingle(raw[8..12])),
                value =>
                {
                    var cast = (PoolChemical)value;
                    return ConcatFields(cast.Buoyancy.ToString(),
                        cast.Viscosity.ToString(), cast.Health.ToString());
                },
                raw =>
                {
                    var parts = SplitFields(raw);
                    return new PoolChemical(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                });
        }

        public static LcsInfo PortInfo() {
            return new LcsInfo(3, "prt", 0x34,
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

        // TODO: REPLACE CONCAT STUFF WITH STRING.JOIN
        public static string ConcatProps(params string[] items)
        {
            return ConcatList(items, PropertySeparator);
        }

        public static string[] SplitProps(string items)
        {
            return SplitList(items, PropertySeparator);
        }

        public static string ConcatFields(params string[] items)
        {
            return ConcatList(items, FieldSeparator);
        }

        public static string[] SplitFields(string items)
        {
            return SplitList(items, FieldSeparator);
        }

        public static string ConcatList(string[] items, string separator)
        {
            return items.Length > 0
                ? items.Aggregate("", (current, property) => current + $"{separator}{property}")[1..] : "";
        }

        public static string[] SplitList(string items, string separator)
        {
            return items.Length > 0 ? items.Split(separator) : Array.Empty<string>();
        }
    }
}
