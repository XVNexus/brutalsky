using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Object;
using Utils.Pool;
using Utils.Shape;

namespace Utils.Lcs
{
    public static class Binifier
    {
        public static byte[] Binify(LcsType type, object value)
        {
            return type switch
            {
                LcsType.Bool => BinifyBool((bool)value),
                LcsType.UShort => BinifyUShort((ushort)value),
                LcsType.UInt => BinifyUInt((uint)value),
                LcsType.ULong => BinifyULong((ulong)value),
                LcsType.Short => BinifyShort((short)value),
                LcsType.Int => BinifyInt((int)value),
                LcsType.Long => BinifyLong((long)value),
                LcsType.Float => BinifyFloat((float)value),
                LcsType.Double => BinifyDouble((double)value),
                LcsType.Char => BinifyChar((char)value),
                LcsType.String => BinifyString((string)value),
                LcsType.Direction => BinifyDirection((Direction)value),
                LcsType.Layer => BinifyLayer((ObjectLayer)value),
                LcsType.FormType => BinifyFormType((FormType)value),
                LcsType.JointType => BinifyJointType((JointType)value),
                LcsType.Vector2 => BinifyVector2((Vector2)value),
                LcsType.Color => BinifyColor((Color)value),
                LcsType.Transform => BinifyTransform((ObjectTransform)value),
                LcsType.Form => BinifyForm((Form)value),
                LcsType.Material => BinifyMaterial((ShapeMaterial)value),
                LcsType.Chemical => BinifyChemical((PoolChemical)value),
                _ => throw Errors.InvalidItem("LCS type", type)
            };
        }

        public static object Parse(LcsType type, byte[] raw)
        {
            return type switch
            {
                LcsType.Bool => ParseBool(raw),
                LcsType.UShort => ParseUShort(raw),
                LcsType.UInt => ParseUInt(raw),
                LcsType.ULong => ParseULong(raw),
                LcsType.Short => ParseShort(raw),
                LcsType.Int => ParseInt(raw),
                LcsType.Long => ParseLong(raw),
                LcsType.Float => ParseFloat(raw),
                LcsType.Double => ParseDouble(raw),
                LcsType.Char => ParseChar(raw),
                LcsType.String => ParseString(raw),
                LcsType.Direction => ParseDirection(raw),
                LcsType.Layer => ParseLayer(raw),
                LcsType.FormType => ParseFormType(raw),
                LcsType.JointType => ParseJointType(raw),
                LcsType.Vector2 => ParseVector2(raw),
                LcsType.Color => ParseColor(raw),
                LcsType.Transform => ParseTransform(raw),
                LcsType.Form => ParseForm(raw),
                LcsType.Material => ParseMaterial(raw),
                LcsType.Chemical => ParseChemical(raw),
                _ => throw Errors.InvalidItem("LCS type", type)
            };
        }

        // Primitive types
        private static byte[] BinifyBool(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        private static bool ParseBool(byte[] raw)
        {
            return BitConverter.ToBoolean(raw);
        }

        private static byte[] BinifyUShort(ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        private static ushort ParseUShort(byte[] raw)
        {
            return BitConverter.ToUInt16(raw);
        }

        private static byte[] BinifyUInt(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        private static uint ParseUInt(byte[] raw)
        {
            return BitConverter.ToUInt32(raw);
        }

        private static byte[] BinifyULong(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        private static ulong ParseULong(byte[] raw)
        {
            return BitConverter.ToUInt64(raw);
        }

        private static byte[] BinifyShort(short value)
        {
            return BitConverter.GetBytes(value);
        }

        private static short ParseShort(byte[] raw)
        {
            return BitConverter.ToInt16(raw);
        }

        private static byte[] BinifyInt(int value)
        {
            return BitConverter.GetBytes(value);
        }

        private static int ParseInt(byte[] raw)
        {
            return BitConverter.ToInt32(raw);
        }

        private static byte[] BinifyLong(long value)
        {
            return BitConverter.GetBytes(value);
        }

        private static long ParseLong(byte[] raw)
        {
            return BitConverter.ToInt64(raw);
        }

        private static byte[] BinifyFloat(float value)
        {
            return BitConverter.GetBytes(value);
        }

        private static float ParseFloat(byte[] raw)
        {
            return BitConverter.ToSingle(raw);
        }

        private static byte[] BinifyDouble(double value)
        {
            return BitConverter.GetBytes(value);
        }

        private static double ParseDouble(byte[] raw)
        {
            return BitConverter.ToDouble(raw);
        }

        private static byte[] BinifyChar(char value)
        {
            return BitConverter.GetBytes(value);
        }

        private static char ParseChar(byte[] raw)
        {
            return BitConverter.ToChar(raw);
        }

        private static byte[] BinifyString(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        private static string ParseString(byte[] raw)
        {
            return Encoding.UTF8.GetString(raw);
        }

        // Enum types
        private static byte[] BinifyDirection(Direction direction)
        {
            return new[] { (byte)direction };
        }

        private static Direction ParseDirection(byte[] raw)
        {
            return (Direction)raw[0];
        }

        private static byte[] BinifyLayer(ObjectLayer layer)
        {
            return new[] { (byte)layer };
        }

        private static ObjectLayer ParseLayer(byte[] raw)
        {
            return (ObjectLayer)raw[0];
        }

        private static byte[] BinifyFormType(FormType formType)
        {
            return new[] { (byte)formType };
        }

        private static FormType ParseFormType(byte[] raw)
        {
            return (FormType)raw[0];
        }

        private static byte[] BinifyJointType(JointType jointType)
        {
            return new[] { (byte)jointType };
        }

        private static JointType ParseJointType(byte[] raw)
        {
            return (JointType)raw[0];
        }

        // Compound types
        private static byte[] BinifyVector2(Vector2 value)
        {
            return BinifyFloat(value.x).Concat(BinifyFloat(value.y)).ToArray();
        }

        private static Vector2 ParseVector2(byte[] raw)
        {
            return new Vector2(ParseFloat(raw[..4]), ParseFloat(raw[4..8]));
        }

        private static byte[] BinifyColor(Color color)
        {
            return new[] { Float01ToByte(color.r), Float01ToByte(color.g),
                Float01ToByte(color.b), Float01ToByte(color.a) };
        }

        private static Color ParseColor(byte[] raw)
        {
            return new Color(ByteToFloat01(raw[0]), ByteToFloat01(raw[1]),
                ByteToFloat01(raw[2]), ByteToFloat01(raw[3]));
        }

        private static byte[] BinifyTransform(ObjectTransform transform)
        {
            return BinifyFloat(transform.Position.x).Concat(BinifyFloat(transform.Position.y))
                .Concat(BinifyFloat(transform.Rotation)).ToArray();
        }

        private static ObjectTransform ParseTransform(byte[] raw)
        {
            return new ObjectTransform(ParseFloat(raw[..4]), ParseFloat(raw[4..8]), ParseFloat(raw[8..12]));
        }

        private static byte[] BinifyForm(Form form)
        {
            var result = new byte[form.Args.Length * 4 + 1];
            result[0] = BinifyFormType(form.Type)[0];
            for (var i = 0; i < form.Args.Length; i++)
            {
                var argBytes = BinifyFloat(form.Args[i]);
                var bitIndex = i * 4 + 1;
                for (var j = 0; j < 4; j++)
                {
                    result[bitIndex + j] = argBytes[j];
                }
            }
            return result;
        }

        private static Form ParseForm(byte[] raw)
        {
            var type = ParseFormType(raw[..1]);
            var parts = new float[(raw.Length - 1) / 4];
            for (var i = 0; i < parts.Length; i++)
            {
                var bitIndex = i * 4 + 1;
                parts[i] = ParseFloat(raw[bitIndex..(bitIndex + 4)]);
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
        }

        private static byte[] BinifyMaterial(ShapeMaterial material)
        {
            return BinifyFloat(material.Friction).Concat(BinifyFloat(material.Restitution))
                .Concat(BinifyFloat(material.Adhesion)).Concat(BinifyFloat(material.Density))
                .Concat(BinifyFloat(material.Health)).ToArray();
        }

        private static ShapeMaterial ParseMaterial(byte[] raw)
        {
            return new ShapeMaterial(ParseFloat(raw[..4]), ParseFloat(raw[4..8]),
                ParseFloat(raw[8..12]), ParseFloat(raw[12..16]), ParseFloat(raw[16..20]));
        }

        private static byte[] BinifyChemical(PoolChemical chemical)
        {
            return BinifyFloat(chemical.Buoyancy).Concat(BinifyFloat(chemical.Viscosity))
                .Concat(BinifyFloat(chemical.Health)).ToArray();
        }

        private static PoolChemical ParseChemical(byte[] raw)
        {
            return new PoolChemical(ParseFloat(raw[..4]), ParseFloat(raw[4..8]), ParseFloat(raw[8..12]));
        }

        // Utility functions
        private static byte Float01ToByte(float value)
        {
            return (byte)(value * 255);
        }

        private static float ByteToFloat01(byte value)
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
    }
}
