using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Object;
using Utils.Pool;
using Utils.Shape;
using ColorUtility = UnityEngine.ColorUtility;

namespace Utils.Lcs
{
    public static class LcsParser
    {
        public const char HeaderSeparator = ':';
        public const char PropertySeperator = ';';
        public const char FieldSeperator = ',';

        // Basic types
        public static string Stringify(bool value)
        {
            return value ? "1" : "";
        }

        public static bool ParseBool(string raw)
        {
            return raw == "1";
        }

        public static string Stringify(int value)
        {
            return value != 0 ? value.ToString() : "";
        }

        public static int ParseInt(string raw)
        {
            return raw.Length > 0 ? int.Parse(raw) : 0;
        }

        public static string Stringify(float value)
        {
            return value switch
            {
                0f => "",
                float.NegativeInfinity => "-.",
                float.PositiveInfinity => ".",
                _ => value.ToString()
            };
        }

        public static float ParseFloat(string raw)
        {
            return raw switch
            {
                "" => 0f,
                "-." => float.NegativeInfinity,
                "." => float.PositiveInfinity,
                _ => float.Parse(raw)
            };
        }

        public static string Stringify(char value)
        {
            return Stringify(value.ToString());
        }

        public static char ParseChar(string raw)
        {
            return ParseString(raw)[0];
        }

        public static string Stringify(string value)
        {
            var result = value;

            // Escape backslashes
            result = result.Replace(@"\", @"\\");

            // Escape whitespace and special characters
            var specialChars = new Dictionary<char, char>
            {
                {FieldSeperator, 'f'},
                {PropertySeperator, 'p'},
                {HeaderSeparator, 'h'},
                {' ', 's'},
                {'\t', 't'},
                {'\n', 'n'},
                {'#', 'g'}
            };
            result = specialChars.Keys.Aggregate(result, (current, specialChar)
                => current.Replace($"{specialChar}", $@"\{specialChars[specialChar]}"));

            return result;
        }

        public static string ParseString(string raw)
        {
            var result = raw;

            // Unescape whitespace and special characters
            result = result.Replace(@"\\", @"\\ ");
            var specialChars = new Dictionary<char, char>
            {
                {'f', FieldSeperator},
                {'p', PropertySeperator},
                {'h', HeaderSeparator},
                {'s', ' '},
                {'t', '\t'},
                {'n', '\n'},
                {'g', '#'}
            };
            result = specialChars.Keys.Aggregate(result, (current, specialChar)
                => current.Replace($@"\{specialChar}", $"{specialChars[specialChar]}"));

            // Unescape backslashes
            result = result.Replace(@"\\ ", @"\");

            return result;
        }

        public static string Stringify(Vector2 value)
        {
            return CompressFields(new[]
            {
                Stringify(value.x),
                Stringify(value.y)
            });
        }

        public static Vector2 ParseVector2(string raw)
        {
            var parts = ExpandFields(raw);
            return new Vector2(ParseFloat(parts[0]), ParseFloat(parts[1]));
        }

        public static string Stringify(Direction direction)
        {
            return Stringify((int)direction);
        }

        public static Direction ParseDirection(string raw)
        {
            return (Direction)ParseInt(raw);
        }

        public static string Stringify(ObjectColor color)
        {
            return (color.Alpha < 1f ? ColorUtility.ToHtmlStringRGBA(color.Color)
                : ColorUtility.ToHtmlStringRGB(color.Color)) + Stringify(color.Glow);
        }

        public static ObjectColor ParseColor(string raw)
        {
            var hexString = raw[..^(raw.Length % 2)];
            ColorUtility.TryParseHtmlString('#' + raw[..^(raw.Length % 2)], out var tint);
            var glow = ParseBool(raw[hexString.Length..]);
            return new ObjectColor(tint, glow);
        }

        public static string Stringify(ObjectLayer layer)
        {
            return Stringify((int)layer);
        }

        public static ObjectLayer ParseLayer(string raw)
        {
            return (ObjectLayer)ParseInt(raw);
        }

        public static string Stringify(ObjectTransform transform)
        {
            return CompressFields(new[]
            {
                Stringify(transform.Position.x),
                Stringify(transform.Position.y),
                Stringify(transform.Rotation)
            });
        }

        public static ObjectTransform ParseTransform(string raw)
        {
            var parts = ExpandFields(raw);
            return new ObjectTransform(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]));
        }

        public static string Stringify(Form form)
        {
            return Stringify((int)form.FormType) + (' ' + form.FormString).Replace(" 0", " ")
                .Replace(' ', FieldSeperator);
        }

        public static Form ParseForm(string raw)
        {
            raw = Regex.Replace(' ' + raw.Replace(FieldSeperator, ' '), " (?= |$)", " 0")[1..];
            var type = (FormType)ParseInt(raw[..1]);
            var parts = raw[2..].Split(' ').Select(part => float.Parse(part)).ToArray();
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

        public static string Stringify(ShapeMaterial material)
        {
            var result = CompressFields(new[]
            {
                Stringify(material.Friction),
                Stringify(material.Restitution),
                Stringify(material.Adhesion),
                Stringify(material.Density),
                Stringify(material.Health),
                Stringify(material.Dynamic)
            });
            return result.Length > 5 ? result : "";
        }

        public static ShapeMaterial ParseMaterial(string raw)
        {
            if (raw.Length == 0)
            {
                return new ShapeMaterial(0f, 0f, 0f, 0f);
            }
            var parts = ExpandFields(raw);
            var friction = ParseFloat(parts[0]);
            var restitution = ParseFloat(parts[1]);
            var adhesion = ParseFloat(parts[2]);
            var density = ParseFloat(parts[3]);
            var health = ParseFloat(parts[4]);
            var dynamic = ParseBool(parts[5]);
            return new ShapeMaterial(friction, restitution, adhesion, density, health, dynamic);
        }

        public static string Stringify(PoolChemical chemical)
        {
            var result = CompressFields(new[]
            {
                Stringify(chemical.Buoyancy),
                Stringify(chemical.Viscosity),
                Stringify(chemical.Health)
            });
            return result.Length > 2 ? result : "";
        }

        public static PoolChemical ParseChemical(string raw)
        {
            if (raw.Length == 0)
            {
                return new PoolChemical(0f, 0f);
            }
            var parts = ExpandFields(raw);
            var buoyancy = ParseFloat(parts[0]);
            var viscosity = ParseFloat(parts[1]);
            var health = ParseFloat(parts[2]);
            return new PoolChemical(buoyancy, viscosity, health);
        }

        public static string Stringify(JointType jointType)
        {
            return Stringify((int)jointType);
        }

        public static JointType ParseJointType(string raw)
        {
            return (JointType)ParseInt(raw);
        }

        // Utilities
        public static string CompressList(string[] items, char separator)
        {
            return items.Aggregate("", (current, property) => current + $"{separator}{property}")[1..];
        }

        public static string[] ExpandList(string items, char separator)
        {
            return items.Split(separator);
        }

        public static string CompressProperties(string[] items)
        {
            return CompressList(items, PropertySeperator);
        }

        public static string[] ExpandProperties(string items)
        {
            return ExpandList(items, PropertySeperator);
        }

        public static string CompressFields(string[] items)
        {
            return CompressList(items, FieldSeperator);
        }

        public static string[] ExpandFields(string items)
        {
            return ExpandList(items, FieldSeperator);
        }
    }
}
