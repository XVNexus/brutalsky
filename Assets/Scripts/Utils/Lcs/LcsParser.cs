using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils.Joint;
using Utils.Object;
using Utils.Pool;
using Utils.Shape;
using ColorUtility = UnityEngine.ColorUtility;
using JointLimits = Utils.Joint.JointLimits;
using JointMotor = Utils.Joint.JointMotor;

namespace Utils.Lcs
{
    public static class LcsParser
    {
        public const int Version = 1;
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
            return value != 0f ? value.ToString() : "";
        }

        public static float ParseFloat(string raw)
        {
            return raw.Length > 0 ? float.Parse(raw) : 0f;
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

        // Unity types
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

        // Object types
        public static string Stringify(ObjectColor color)
        {
            return (color.Alpha < 1f ? ColorUtility.ToHtmlStringRGBA(color.Tint)
                : ColorUtility.ToHtmlStringRGB(color.Tint)) + Stringify(color.Glow);
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

        // Shape types
        public static string Stringify(Form form)
        {
            return Stringify((int)form.FormType) + (' ' + form.FormString).Replace(" 0", " ")
                .Replace(' ', FieldSeperator);
        }

        public static Form ParseForm(string raw)
        {
            Form result;
            var rawFull = Regex.Replace(' ' + raw.Replace(FieldSeperator, ' '),
                " (?= |$)", " 0")[1..];
            var formType = (FormType)ParseInt(rawFull[..1]);
            var formData = rawFull[2..];
            var formParts = formData.Split(' ');
            switch (formType)
            {
                case FormType.Vector:
                    result = Form.Vector(formData);
                    break;
                case FormType.Polygon:
                    List<Vector2> polygonPoints = new();
                    for (var i = 0; i < formParts.Length; i += 2)
                    {
                        polygonPoints.Add(new Vector2(float.Parse(formParts[i]), float.Parse(formParts[i + 1])));
                    }
                    result = Form.Polygon(polygonPoints.ToArray());
                    break;
                case FormType.Square:
                    result = Form.Square(float.Parse(formParts[0]));
                    break;
                case FormType.Rectangle:
                    result = Form.Rectangle(float.Parse(formParts[0]), float.Parse(formParts[1]));
                    break;
                case FormType.Circle:
                    result = Form.Circle(float.Parse(formParts[0]));
                    break;
                case FormType.Ellipse:
                    result = Form.Ellipse(float.Parse(formParts[0]), float.Parse(formParts[1]));
                    break;
                case FormType.Ngon:
                    result = Form.Ngon(int.Parse(formParts[0]), float.Parse(formParts[1]));
                    break;
                case FormType.Star:
                    result = Form.Star(int.Parse(formParts[0]), float.Parse(formParts[1]),
                        float.Parse(formParts[2]));
                    break;
                default:
                    result = Form.Ngon(3, 1f);
                    break;
            }
            return result;
        }

        public static string Stringify(ShapeMaterial material)
        {
            return CompressFields(new[]
            {
                Stringify(material.Friction),
                Stringify(material.Restitution),
                Stringify(material.Adhesion),
                Stringify(material.Density),
                Stringify(material.Health),
                Stringify(material.Dynamic)
            });
        }

        public static ShapeMaterial ParseMaterial(string raw)
        {
            var parts = ExpandFields(raw);
            var friction = ParseFloat(parts[0]);
            var restitution = ParseFloat(parts[1]);
            var adhesion = ParseFloat(parts[2]);
            var density = ParseFloat(parts[3]);
            var health = ParseFloat(parts[4]);
            var dynamic = ParseBool(parts[5]);
            return new ShapeMaterial(friction, restitution, adhesion, density, health, dynamic);
        }

        // Pool types
        public static string Stringify(PoolChemical chemical)
        {
            return CompressFields(new[]
            {
                Stringify(chemical.Buoyancy),
                Stringify(chemical.Viscosity),
                Stringify(chemical.Health)
            });
        }

        public static PoolChemical ParseChemical(string raw)
        {
            var parts = ExpandFields(raw);
            var buoyancy = ParseFloat(parts[0]);
            var viscosity = ParseFloat(parts[1]);
            var health = ParseFloat(parts[2]);
            return new PoolChemical(buoyancy, viscosity, health);
        }

        // Joint types
        public static string Stringify(JointType jointType)
        {
            return Stringify((int)jointType);
        }

        public static JointType ParseJointType(string raw)
        {
            return (JointType)ParseInt(raw);
        }

        public static string Stringify(JointConfig jointConfig)
        {
            return jointConfig.Auto ? "" : jointConfig.Value.ToString();
        }

        public static JointConfig ParseJointConfig(string raw)
        {
            return raw == "" ? JointConfig.AutoValue() : JointConfig.SetValue(ParseFloat(raw));
        }

        public static string Stringify(JointDamping jointDamping)
        {
            return CompressFields(new[]
            {
                Stringify(jointDamping.Ratio),
                Stringify(jointDamping.Frequency)
            });
        }

        public static JointDamping ParseJointDamping(string raw)
        {
            var parts = ExpandFields(raw);
            return JointDamping.Damped(ParseFloat(parts[0]), ParseFloat(parts[1]));
        }

        public static string Stringify(JointLimits jointLimits)
        {
            return jointLimits.Use ? CompressFields(new[]
            {
                Stringify(jointLimits.Min),
                Stringify(jointLimits.Max)
            }) : "";
        }

        public static JointLimits ParseJointLimits(string raw)
        {
            if (raw == "") return JointLimits.Unlimited();
            var parts = ExpandFields(raw);
            return JointLimits.Limited(ParseFloat(parts[0]), ParseFloat(parts[1]));
        }

        public static string Stringify(JointMotor jointMotor)
        {
            return jointMotor.Use ? CompressFields(new[]
            {
                Stringify(jointMotor.Speed),
                Stringify(jointMotor.MaxForce)
            }) : "";
        }

        public static JointMotor ParseJointMotor(string raw)
        {
            if (raw == "") return JointMotor.Unpowered();
            var parts = ExpandFields(raw);
            return JointMotor.Powered(ParseFloat(parts[0]), ParseFloat(parts[1]));
        }

        public static string Stringify(JointStrength jointStrength)
        {
            var breakable = !float.IsPositiveInfinity(jointStrength.BreakForce)
                            || !float.IsPositiveInfinity(jointStrength.BreakTorque);
            return breakable ? CompressFields(new[]
            {
                Stringify(jointStrength.BreakForce),
                Stringify(jointStrength.BreakTorque)
            }) : "";
        }

        public static JointStrength ParseJointStrength(string raw)
        {
            if (raw == "") return JointStrength.Unbreakable();
            var parts = ExpandFields(raw);
            return JointStrength.Breakable(ParseFloat(parts[0]), ParseFloat(parts[1]));
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
