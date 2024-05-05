using System;
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
        // Primitive types
        public static byte[] Binify(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public static bool ParseBool(byte[] raw)
        {
            return BitConverter.ToBoolean(raw);
        }

        public static byte[] Binify(ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        public static ushort ParseUShort(byte[] raw)
        {
            return BitConverter.ToUInt16(raw);
        }

        public static byte[] Binify(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        public static uint ParseUInt(byte[] raw)
        {
            return BitConverter.ToUInt32(raw);
        }

        public static byte[] Binify(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        public static ulong ParseULong(byte[] raw)
        {
            return BitConverter.ToUInt64(raw);
        }

        public static byte[] Binify(short value)
        {
            return BitConverter.GetBytes(value);
        }

        public static short ParseShort(byte[] raw)
        {
            return BitConverter.ToInt16(raw);
        }

        public static byte[] Binify(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static int ParseInt(byte[] raw)
        {
            return BitConverter.ToInt32(raw);
        }

        public static byte[] Binify(long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static long ParseLong(byte[] raw)
        {
            return BitConverter.ToInt64(raw);
        }

        public static byte[] Binify(float value)
        {
            return BitConverter.GetBytes(value);
        }

        public static float ParseFloat(byte[] raw)
        {
            return BitConverter.ToSingle(raw);
        }

        public static byte[] Binify(double value)
        {
            return BitConverter.GetBytes(value);
        }

        public static double ParseDouble(byte[] raw)
        {
            return BitConverter.ToDouble(raw);
        }

        public static byte[] Binify(char value)
        {
            return BitConverter.GetBytes(value);
        }

        public static char ParseChar(byte[] raw)
        {
            return BitConverter.ToChar(raw);
        }

        public static byte[] Binify(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ParseString(byte[] raw)
        {
            return Encoding.UTF8.GetString(raw);
        }

        // Enum types
        public static byte Binify(Direction direction)
        {
            return (byte)direction;
        }

        public static Direction ParseDirection(byte raw)
        {
            return (Direction)raw;
        }

        public static byte Binify(ObjectLayer layer)
        {
            return (byte)layer;
        }

        public static ObjectLayer ParseLayer(byte raw)
        {
            return (ObjectLayer)raw;
        }

        public static byte Binify(FormType formType)
        {
            return (byte)formType;
        }

        public static FormType ParseFormType(byte raw)
        {
            return (FormType)raw;
        }

        public static byte Binify(JointType jointType)
        {
            return (byte)jointType;
        }

        public static JointType ParseJointType(byte raw)
        {
            return (JointType)raw;
        }

        // Compound types
        public static byte[] Binify(Vector2 value)
        {
            return Binify(value.x).Concat(Binify(value.y)).ToArray();
        }

        public static Vector2 ParseVector2(byte[] raw)
        {
            return new Vector2(ParseFloat(raw[..4]), ParseFloat(raw[4..8]));
        }

        public static byte[] Binify(Color color)
        {
            return new[] { Float01ToByte(color.r), Float01ToByte(color.g),
                Float01ToByte(color.b), Float01ToByte(color.a) };
        }

        public static Color ParseColor(byte[] raw)
        {
            return new Color(ByteToFloat01(raw[0]), ByteToFloat01(raw[1]),
                ByteToFloat01(raw[2]), ByteToFloat01(raw[3]));
        }

        public static byte[] Binify(ObjectTransform transform)
        {
            return Binify(transform.Position.x).Concat(Binify(transform.Position.y))
                .Concat(Binify(transform.Rotation)).ToArray();
        }

        public static ObjectTransform ParseTransform(byte[] raw)
        {
            return new ObjectTransform(ParseFloat(raw[..4]), ParseFloat(raw[4..8]), ParseFloat(raw[8..12]));
        }

        public static byte[] Binify(Form form)
        {
            var result = new byte[form.Args.Length * 4 + 1];
            result[0] = Binify(form.Type);
            for (var i = 0; i < form.Args.Length; i++)
            {
                var argBytes = Binify(form.Args[i]);
                var bitIndex = i * 4 + 1;
                for (var j = 0; j < 4; j++)
                {
                    result[bitIndex + j] = argBytes[j];
                }
            }
            return result;
        }

        public static Form ParseForm(byte[] raw)
        {
            var type = ParseFormType(raw[0]);
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

        public static byte[] Binify(ShapeMaterial material)
        {
            return Binify(material.Friction).Concat(Binify(material.Restitution))
                .Concat(Binify(material.Adhesion)).Concat(Binify(material.Density))
                .Concat(Binify(material.Health)).ToArray();
        }

        public static ShapeMaterial ParseMaterial(byte[] raw)
        {
            return new ShapeMaterial(ParseFloat(raw[..4]), ParseFloat(raw[4..8]),
                ParseFloat(raw[8..12]), ParseFloat(raw[12..16]), ParseFloat(raw[16..20]));
        }

        public static byte[] Binify(PoolChemical chemical)
        {
            return Binify(chemical.Buoyancy).Concat(Binify(chemical.Viscosity))
                .Concat(Binify(chemical.Health)).ToArray();
        }

        public static PoolChemical ParseChemical(byte[] raw)
        {
            return new PoolChemical(ParseFloat(raw[..4]), ParseFloat(raw[4..8]), ParseFloat(raw[8..12]));
        }

        // Utility functions
        private static byte IntToByte(int value)
        {
            return BitConverter.GetBytes(value)[0];
        }

        private static int ByteToInt(byte value)
        {
            return value;
        }

        private static byte Float01ToByte(float value)
        {
            return IntToByte((int)(value * 255));
        }

        private static float ByteToFloat01(byte value)
        {
            return ByteToInt(value) / 255f;
        }
    }
}
