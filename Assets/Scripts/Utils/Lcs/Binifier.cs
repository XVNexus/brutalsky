using System;
using System.Linq;
using System.Text;
using Brutalsky.Logic;
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
        public static byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public static bool ToBoolean(byte[] raw)
        {
            return BitConverter.ToBoolean(raw);
        }

        public static byte[] GetBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static int ToInt32(byte[] raw)
        {
            return BitConverter.ToInt32(raw);
        }

        public static byte[] GetBytes(float value)
        {
            return BitConverter.GetBytes(value);
        }

        public static float ToSingle(byte[] raw)
        {
            return BitConverter.ToSingle(raw);
        }

        public static byte[] GetBytes(char value)
        {
            return BitConverter.GetBytes(value);
        }

        public static char ToChar(byte[] raw)
        {
            return BitConverter.ToChar(raw);
        }

        public static byte[] GetBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ToString(byte[] raw)
        {
            return Encoding.UTF8.GetString(raw);
        }

        public static byte[] GetBytes(Vector2 value)
        {
            return GetBytes(value.x).Concat(GetBytes(value.y)).ToArray();
        }

        public static Vector2 ToVector2(byte[] raw)
        {
            return new Vector2(ToSingle(raw[..4]), ToSingle(raw[4..8]));
        }

        public static byte[] GetBytes(Direction direction)
        {
            return new[] { (byte)direction };
        }

        public static Direction ToDirection(byte[] raw)
        {
            return (Direction)raw[0];
        }

        public static byte[] GetBytes(Color color)
        {
            return new[] { Float01ToByte(color.r), Float01ToByte(color.g),
                Float01ToByte(color.b), Float01ToByte(color.a) };
        }

        public static Color ToColor(byte[] raw)
        {
            return new Color(ByteToFloat01(raw[0]), ByteToFloat01(raw[1]),
                ByteToFloat01(raw[2]), ByteToFloat01(raw[3]));
        }

        public static byte[] GetBytes(ObjectLayer layer)
        {
            return new[] { (byte)layer };
        }

        public static ObjectLayer ToLayer(byte[] raw)
        {
            return (ObjectLayer)raw[0];
        }

        public static byte[] GetBytes(ObjectTransform transform)
        {
            return GetBytes(transform.Position.x).Concat(GetBytes(transform.Position.y))
                .Concat(GetBytes(transform.Rotation)).ToArray();
        }

        public static ObjectTransform ToTransform(byte[] raw)
        {
            return new ObjectTransform(ToSingle(raw[..4]), ToSingle(raw[4..8]), ToSingle(raw[8..12]));
        }

        public static byte[] GetBytes(Form form)
        {
            var result = new byte[form.Args.Length * 4 + 1];
            result[0] = (byte)form.Type;
            for (var i = 0; i < form.Args.Length; i++)
            {
                var argBytes = GetBytes(form.Args[i]);
                var bitIndex = i * 4 + 1;
                for (var j = 0; j < 4; j++)
                {
                    result[bitIndex + j] = argBytes[j];
                }
            }
            return result;
        }

        public static Form ToForm(byte[] raw)
        {
            var type = (FormType)raw[0];
            var parts = new float[(raw.Length - 1) / 4];
            for (var i = 0; i < parts.Length; i++)
            {
                var bitIndex = i * 4 + 1;
                parts[i] = ToSingle(raw[bitIndex..(bitIndex + 4)]);
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

        public static byte[] GetBytes(ShapeMaterial material)
        {
            return GetBytes(material.Friction).Concat(GetBytes(material.Restitution))
                .Concat(GetBytes(material.Adhesion)).Concat(GetBytes(material.Density))
                .Concat(GetBytes(material.Health)).ToArray();
        }

        public static ShapeMaterial ToMaterial(byte[] raw)
        {
            return new ShapeMaterial(ToSingle(raw[..4]), ToSingle(raw[4..8]),
                ToSingle(raw[8..12]), ToSingle(raw[12..16]), ToSingle(raw[16..20]));
        }

        public static byte[] GetBytes(PoolChemical chemical)
        {
            return GetBytes(chemical.Buoyancy).Concat(GetBytes(chemical.Viscosity))
                .Concat(GetBytes(chemical.Health)).ToArray();
        }

        public static PoolChemical ToChemical(byte[] raw)
        {
            return new PoolChemical(ToSingle(raw[..4]), ToSingle(raw[4..8]), ToSingle(raw[8..12]));
        }

        public static byte[] GetBytes(JointType jointType)
        {
            return new[] { (byte)jointType };
        }

        public static JointType ToJointType(byte[] raw)
        {
            return (JointType)raw[0];
        }

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
