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

        public static byte[] GetBytes(BsNode node)
        {
            var result = new byte[node.Config.Length * 4 + 4];
            var tagBytes = GetBytes(node.Tag);
            for (var i = 0; i < 4; i++)
            {
                result[i] = tagBytes[i];
            }
            for (var i = 0; i < node.Config.Length; i++)
            {
                var argBytes = GetBytes(node.Config[i]);
                var bitIndex = i * 4 + 4;
                for (var j = 0; j < 4; j++)
                {
                    result[bitIndex + j] = argBytes[j];
                }
            }
            return result;
        }

        public static BsNode ToNode(byte[] raw)
        {
            var tag = ToString(raw[..4]);
            var config = new float[raw.Length / 4 - 1];
            for (var i = 0; i < config.Length; i++)
            {
                var bitIndex = i * 4 + 4;
                config[i] = ToSingle(raw[bitIndex..(bitIndex + 4)]);
            }
            return tag switch
            {
                // Basic nodes
                "bcbl" => BsNode.ConstantBool(BsMatrix.ToBool(config[0])),
                "bcin" => BsNode.ConstantInt(BsMatrix.ToInt(config[0])),
                "bcfl" => BsNode.ConstantFloat(config[0]),
                "brbl" => BsNode.RandomBool(),
                "brin" => BsNode.RandomInt(BsMatrix.ToInt(config[0]), BsMatrix.ToInt(config[1])),
                "brfl" => BsNode.RandomFloat(config[0], config[1]),
                "bmem" => BsNode.MemoryCell(),
                // Flow nodes
                "ftmr" => BsNode.GameTime(),
                "fdly" => BsNode.Delayer(BsMatrix.ToInt(config[0])),
                "fclk" => BsNode.Clock(BsMatrix.ToInt(config[0])),
                "fchl" => BsNode.ChangeListener(),
                "fmux" => BsNode.Multiplexer(BsMatrix.ToInt(config[0])),
                "fdmx" => BsNode.Demultiplexer(BsMatrix.ToInt(config[0])),
                // Logic nodes
                "lbuf" => BsNode.Buffer(),
                "lnot" => BsNode.Not(),
                "land" => BsNode.And(BsMatrix.ToInt(config[0])),
                "lgor" => BsNode.Or(BsMatrix.ToInt(config[0])),
                "lxor" => BsNode.Xor(BsMatrix.ToInt(config[0])),
                "lnan" => BsNode.Nand(BsMatrix.ToInt(config[0])),
                "lnor" => BsNode.Nor(BsMatrix.ToInt(config[0])),
                "lxnr" => BsNode.Xnor(BsMatrix.ToInt(config[0])),
                // Math nodes
                "madd" => BsNode.Add(BsMatrix.ToInt(config[0])),
                "msub" => BsNode.Subtract(BsMatrix.ToInt(config[0])),
                "mmul" => BsNode.Multiply(BsMatrix.ToInt(config[0])),
                "mdiv" => BsNode.Divide(BsMatrix.ToInt(config[0])),
                "mpow" => BsNode.Pow(),
                "mrot" => BsNode.Root(),
                "msin" => BsNode.Sin(),
                "mcos" => BsNode.Cos(),
                "mtan" => BsNode.Tan(),
                "masn" => BsNode.Asin(),
                "macs" => BsNode.Acos(),
                "matn" => BsNode.Atan(),
                _ => throw Errors.InvalidItem("node tag", tag)
            };
        }

        public static byte[] GetBytes(BsLink link)
        {
            return GetBytes(link.FromPort.Item1).Concat(GetBytes(link.FromPort.Item2))
                .Concat(GetBytes(link.ToPort.Item1)).Concat(GetBytes(link.ToPort.Item2)).ToArray();
        }

        public static BsLink ToLink(byte[] raw)
        {
            return new BsLink(ToInt32(raw[..4]), ToInt32(raw[4..8]),
                ToInt32(raw[8..12]), ToInt32(raw[12..16]));
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
