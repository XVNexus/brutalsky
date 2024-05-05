using System;
using System.Collections.Generic;
using System.Linq;
using Brutalsky.Logic;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Object;
using Utils.Pool;
using Utils.Shape;

namespace Utils.Lcs
{
    public static class Stringifier
    {
        public static string GetString(bool value)
        {
            return value ? "1" : "0";
        }

        public static bool ToBoolean(string raw)
        {
            return raw[0] == '1';
        }

        public static string GetString(int value)
        {
            return value.ToString();
        }

        public static int ToInt32(string raw)
        {
            return int.Parse(raw);
        }

        public static string GetString(float value)
        {
            return value.ToString();
        }

        public static float ToSingle(string raw)
        {
            return float.Parse(raw);
        }

        public static string GetString(char value)
        {
            return GetString(value.ToString());
        }

        public static char ToChar(string raw)
        {
            return ToString(raw)[0];
        }

        public static string GetString(string value)
        {
            var result = value;

            // Escape backslashes
            result = result.Replace(@"\", @"\\");

            // Escape whitespace and special characters
            var specialChars = new Dictionary<char, char>
            {
                {LcsParser.FieldSeperator, 'f'},
                {LcsParser.PropertySeperator, 'p'},
                {' ', 's'},
                {'\t', 't'},
                {'\n', 'n'}
            };
            result = specialChars.Keys.Aggregate(result, (current, specialChar)
                => current.Replace($"{specialChar}", $@"\{specialChars[specialChar]}"));

            return result;
        }

        public static string ToString(string raw)
        {
            var result = raw;

            // Unescape whitespace and special characters
            result = result.Replace(@"\\", @"\\ ");
            var specialChars = new Dictionary<char, char>
            {
                {'f', LcsParser.FieldSeperator},
                {'p', LcsParser.PropertySeperator},
                {'s', ' '},
                {'t', '\t'},
                {'n', '\n'}
            };
            result = specialChars.Keys.Aggregate(result, (current, specialChar)
                => current.Replace($@"\{specialChar}", $"{specialChars[specialChar]}"));

            // Unescape backslashes
            result = result.Replace(@"\\ ", @"\");

            return result;
        }

        public static string GetString(Vector2 value)
        {
            return LcsParser.CompressFields(new[]
            {
                GetString(value.x),
                GetString(value.y)
            });
        }

        public static Vector2 ToVector2(string raw)
        {
            var parts = LcsParser.ExpandFields(raw);
            return new Vector2(ToSingle(parts[0]), ToSingle(parts[1]));
        }

        public static string GetString(Direction direction)
        {
            return GetString((int)direction);
        }

        public static Direction ToDirection(string raw)
        {
            return (Direction)ToInt32(raw);
        }

        public static string GetString(Color color)
        {
            return Float01ToHex(color.r) + Float01ToHex(color.g) + Float01ToHex(color.b) + Float01ToHex(color.a);
        }

        public static Color ToColor(string raw)
        {
            return new Color(HexToFloat01(raw[..2]), HexToFloat01(raw[2..4]), HexToFloat01(raw[4..6]),
                HexToFloat01(raw[6..8]));
        }

        public static string GetString(ObjectLayer layer)
        {
            return GetString((int)layer);
        }

        public static ObjectLayer ToLayer(string raw)
        {
            return (ObjectLayer)ToInt32(raw);
        }

        public static string GetString(ObjectTransform transform)
        {
            return LcsParser.CompressFields(new[]
            {
                GetString(transform.Position.x),
                GetString(transform.Position.y),
                GetString(transform.Rotation)
            });
        }

        public static ObjectTransform ToTransform(string raw)
        {
            var parts = LcsParser.ExpandFields(raw);
            return new ObjectTransform(ToSingle(parts[0]), ToSingle(parts[1]), ToSingle(parts[2]));
        }

        public static string GetString(Form form)
        {
            return form.Args.Aggregate(GetString((int)form.Type),
                (current, arg) => current + (LcsParser.FieldSeperator + GetString(arg)));
        }

        public static Form ToForm(string raw)
        {
            var type = (FormType)ToInt32(raw[..1]);
            var parts = raw[2..].Split(LcsParser.FieldSeperator).Select(part => float.Parse(part)).ToArray();
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

        public static string GetString(ShapeMaterial material)
        {
            return LcsParser.CompressFields(new[]
            {
                GetString(material.Friction),
                GetString(material.Restitution),
                GetString(material.Adhesion),
                GetString(material.Density),
                GetString(material.Health)
            });
        }

        public static ShapeMaterial ToMaterial(string raw)
        {
            var parts = LcsParser.ExpandFields(raw);
            var friction = ToSingle(parts[0]);
            var restitution = ToSingle(parts[1]);
            var adhesion = ToSingle(parts[2]);
            var density = ToSingle(parts[3]);
            var health = ToSingle(parts[4]);
            return new ShapeMaterial(friction, restitution, adhesion, density, health);
        }

        public static string GetString(PoolChemical chemical)
        {
            return LcsParser.CompressFields(new[]
            {
                GetString(chemical.Buoyancy),
                GetString(chemical.Viscosity),
                GetString(chemical.Health)
            });
        }

        public static PoolChemical ToChemical(string raw)
        {
            var parts = LcsParser.ExpandFields(raw);
            var buoyancy = ToSingle(parts[0]);
            var viscosity = ToSingle(parts[1]);
            var health = ToSingle(parts[2]);
            return new PoolChemical(buoyancy, viscosity, health);
        }

        public static string GetString(JointType jointType)
        {
            return GetString((int)jointType);
        }

        public static JointType ToJointType(string raw)
        {
            return (JointType)ToInt32(raw);
        }

        public static string GetString(BsNode node)
        {
            var result = new string[node.Config.Length + 1];
            result[0] = GetString(node.Tag);
            for (var i = 0; i < node.Config.Length; i++)
            {
                result[i + 1] = GetString(node.Config[i]);
            }
            return LcsParser.CompressFields(result);
        }

        public static BsNode ToNode(string raw)
        {
            var fields = LcsParser.ExpandFields(raw);
            var tag = ToString(fields[0]);
            var config = new float[fields.Length - 1];
            for (var i = 1; i < fields.Length; i++)
            {
                config[i - 1] = ToSingle(fields[i]);
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

        public static string GetString(BsLink link, int hexWidth)
        {
            return IntToHex(link.FromPort.Item1, hexWidth) + IntToHex(link.FromPort.Item2, hexWidth)
                + IntToHex(link.ToPort.Item1, hexWidth) + IntToHex(link.ToPort.Item2, hexWidth);
        }

        public static BsLink ToLink(string raw, int hexWidth)
        {
            return new BsLink(HexToInt(raw[..hexWidth]), HexToInt(raw[hexWidth..(hexWidth * 2)]),
                HexToInt(raw[(hexWidth * 2)..(hexWidth * 3)]), HexToInt(raw[(hexWidth * 3)..(hexWidth * 4)]));
        }

        private static string IntToHex(int value, int digits)
        {
            return value.ToString($"X{digits}");
        }

        private static int HexToInt(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        private static string Float01ToHex(float value)
        {
            return IntToHex((int)(value * 255), 2);
        }

        private static float HexToFloat01(string hex)
        {
            return HexToInt(hex) / 255f;
        }
    }
}
