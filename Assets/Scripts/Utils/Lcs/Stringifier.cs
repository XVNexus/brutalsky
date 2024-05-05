using System;
using System.Collections.Generic;
using System.Linq;
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
        public const char PropertySeperator = ';';
        public const char FieldSeperator = ',';

        // TODO: FOR THE LOVE OF ALL THAT IS HOLY AND UNHOLY THERE HAS TO BE A BETTER WAY TO DO THIS
        public static string Stringify<T>(object value)
        {
            var type = typeof(T);
            return type switch
            {
                not null when type == typeof(bool) => StringifyBool((bool)value),
                not null when type == typeof(ushort) => StringifyUShort((ushort)value),
                not null when type == typeof(uint) => StringifyUInt((uint)value),
                not null when type == typeof(ulong) => StringifyULong((ulong)value),
                not null when type == typeof(short) => StringifyShort((short)value),
                not null when type == typeof(int) => StringifyInt((int)value),
                not null when type == typeof(long) => StringifyLong((long)value),
                not null when type == typeof(float) => StringifyFloat((float)value),
                not null when type == typeof(double) => StringifyDouble((double)value),
                not null when type == typeof(char) => StringifyChar((char)value),
                not null when type == typeof(string) => StringifyString((string)value),
                not null when type == typeof(Direction) => StringifyDirection((Direction)value),
                not null when type == typeof(ObjectLayer) => StringifyObjectLayer((ObjectLayer)value),
                not null when type == typeof(FormType) => StringifyFormType((FormType)value),
                not null when type == typeof(JointType) => StringifyJointType((JointType)value),
                not null when type == typeof(Vector2) => StringifyVector2((Vector2)value),
                not null when type == typeof(Color) => StringifyColor((Color)value),
                not null when type == typeof(ObjectTransform) => StringifyTransform((ObjectTransform)value),
                not null when type == typeof(Form) => StringifyForm((Form)value),
                not null when type == typeof(ShapeMaterial) => StringifyMaterial((ShapeMaterial)value),
                not null when type == typeof(PoolChemical) => StringifyChemical((PoolChemical)value),
                _ => throw Errors.InvalidItem("LCS type", type)
            };
        }

        public static string Str<T>(object value)
        {
            return Stringify<T>(value);
        }

        public static T Parse<T>(string raw)
        {
            var type = typeof(T);
            return (T)(object)(type switch
            {
                not null when type == typeof(bool) => ParseBool(raw),
                not null when type == typeof(ushort) => ParseUShort(raw),
                not null when type == typeof(uint) => ParseUInt(raw),
                not null when type == typeof(ulong) => ParseULong(raw),
                not null when type == typeof(short) => ParseShort(raw),
                not null when type == typeof(int) => ParseInt(raw),
                not null when type == typeof(long) => ParseLong(raw),
                not null when type == typeof(float) => ParseFloat(raw),
                not null when type == typeof(double) => ParseDouble(raw),
                not null when type == typeof(char) => ParseChar(raw),
                not null when type == typeof(string) => ParseString(raw),
                not null when type == typeof(Direction) => ParseDirection(raw),
                not null when type == typeof(ObjectLayer) => ParseLayer(raw),
                not null when type == typeof(FormType) => ParseFormType(raw),
                not null when type == typeof(JointType) => ParseJointType(raw),
                not null when type == typeof(Vector2) => ParseVector2(raw),
                not null when type == typeof(Color) => ParseColor(raw),
                not null when type == typeof(ObjectTransform) => ParseTransform(raw),
                not null when type == typeof(Form) => ParseForm(raw),
                not null when type == typeof(ShapeMaterial) => ParseMaterial(raw),
                not null when type == typeof(PoolChemical) => ParseChemical(raw),
                _ => throw Errors.InvalidItem("LCS type", type)
            });
        }

        public static T Par<T>(string raw)
        {
            return Parse<T>(raw);
        }

        // Primitive types
        private static string StringifyBool(bool value)
        {
            return value ? "1" : "0";
        }

        private static bool ParseBool(string raw)
        {
            return raw[0] == '1';
        }

        private static string StringifyUShort(ushort value)
        {
            return value.ToString();
        }

        private static ushort ParseUShort(string raw)
        {
            return ushort.Parse(raw);
        }

        private static string StringifyUInt(uint value)
        {
            return value.ToString();
        }

        private static uint ParseUInt(string raw)
        {
            return uint.Parse(raw);
        }

        private static string StringifyULong(ulong value)
        {
            return value.ToString();
        }

        private static ulong ParseULong(string raw)
        {
            return ulong.Parse(raw);
        }

        private static string StringifyShort(short value)
        {
            return value.ToString();
        }

        private static short ParseShort(string raw)
        {
            return short.Parse(raw);
        }

        private static string StringifyInt(int value)
        {
            return value.ToString();
        }

        private static int ParseInt(string raw)
        {
            return int.Parse(raw);
        }

        private static string StringifyLong(long value)
        {
            return value.ToString();
        }

        private static long ParseLong(string raw)
        {
            return long.Parse(raw);
        }

        private static string StringifyFloat(float value)
        {
            return value.ToString();
        }

        private static float ParseFloat(string raw)
        {
            return float.Parse(raw);
        }

        private static string StringifyDouble(double value)
        {
            return value.ToString();
        }

        private static double ParseDouble(string raw)
        {
            return double.Parse(raw);
        }

        private static string StringifyChar(char value)
        {
            return StringifyString(value.ToString());
        }

        private static char ParseChar(string raw)
        {
            return ParseString(raw)[0];
        }

        private static string StringifyString(string value)
        {
            var result = value;

            // Escape backslashes
            result = result.Replace(@"\", @"\\");

            // Escape whitespace and special characters
            var specialChars = new Dictionary<char, char>
            {
                {FieldSeperator, 'f'},
                {PropertySeperator, 'p'},
                {' ', 's'},
                {'\t', 't'},
                {'\n', 'n'}
            };
            result = specialChars.Keys.Aggregate(result, (current, specialChar)
                => current.Replace($"{specialChar}", $@"\{specialChars[specialChar]}"));

            return result;
        }

        private static string ParseString(string raw)
        {
            var result = raw;

            // Unescape whitespace and special characters
            result = result.Replace(@"\\", @"\\ ");
            var specialChars = new Dictionary<char, char>
            {
                {'f', FieldSeperator},
                {'p', PropertySeperator},
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

        // Enum types
        private static string StringifyDirection(Direction direction)
        {
            return StringifyInt((int)direction);
        }

        private static Direction ParseDirection(string raw)
        {
            return (Direction)ParseInt(raw);
        }

        private static string StringifyObjectLayer(ObjectLayer layer)
        {
            return StringifyInt((int)layer);
        }

        private static ObjectLayer ParseLayer(string raw)
        {
            return (ObjectLayer)ParseInt(raw);
        }

        private static string StringifyFormType(FormType formType)
        {
            return StringifyInt((int)formType);
        }

        private static FormType ParseFormType(string raw)
        {
            return (FormType)ParseInt(raw);
        }

        private static string StringifyJointType(JointType jointType)
        {
            return StringifyInt((int)jointType);
        }

        private static JointType ParseJointType(string raw)
        {
            return (JointType)ParseInt(raw);
        }

        // Compound types
        private static string StringifyVector2(Vector2 value)
        {
            return CompressFields(new[]
            {
                StringifyFloat(value.x),
                StringifyFloat(value.y)
            });
        }

        private static Vector2 ParseVector2(string raw)
        {
            var parts = ExpandFields(raw);
            return new Vector2(ParseFloat(parts[0]), ParseFloat(parts[1]));
        }


        private static string StringifyColor(Color color)
        {
            return Float01ToHex(color.r) + Float01ToHex(color.g) + Float01ToHex(color.b) + Float01ToHex(color.a);
        }

        private static Color ParseColor(string raw)
        {
            return new Color(HexToFloat01(raw[..2]), HexToFloat01(raw[2..4]), HexToFloat01(raw[4..6]),
                HexToFloat01(raw[6..8]));
        }

        private static string StringifyTransform(ObjectTransform transform)
        {
            return CompressFields(new[]
            {
                StringifyFloat(transform.Position.x),
                StringifyFloat(transform.Position.y),
                StringifyFloat(transform.Rotation)
            });
        }

        private static ObjectTransform ParseTransform(string raw)
        {
            var parts = ExpandFields(raw);
            return new ObjectTransform(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]));
        }

        private static string StringifyForm(Form form)
        {
            return form.Args.Aggregate(StringifyFormType(form.Type),
                (current, arg) => current + (FieldSeperator + StringifyFloat(arg)));
        }

        private static Form ParseForm(string raw)
        {
            var parts = raw.Split(FieldSeperator);
            var type = ParseFormType(parts[0]);
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
        }

        private static string StringifyMaterial(ShapeMaterial material)
        {
            return CompressFields(new[]
            {
                StringifyFloat(material.Friction),
                StringifyFloat(material.Restitution),
                StringifyFloat(material.Adhesion),
                StringifyFloat(material.Density),
                StringifyFloat(material.Health)
            });
        }

        private static ShapeMaterial ParseMaterial(string raw)
        {
            var parts = ExpandFields(raw);
            var friction = ParseFloat(parts[0]);
            var restitution = ParseFloat(parts[1]);
            var adhesion = ParseFloat(parts[2]);
            var density = ParseFloat(parts[3]);
            var health = ParseFloat(parts[4]);
            return new ShapeMaterial(friction, restitution, adhesion, density, health);
        }

        private static string StringifyChemical(PoolChemical chemical)
        {
            return CompressFields(new[]
            {
                StringifyFloat(chemical.Buoyancy),
                StringifyFloat(chemical.Viscosity),
                StringifyFloat(chemical.Health)
            });
        }

        private static PoolChemical ParseChemical(string raw)
        {
            var parts = ExpandFields(raw);
            var buoyancy = ParseFloat(parts[0]);
            var viscosity = ParseFloat(parts[1]);
            var health = ParseFloat(parts[2]);
            return new PoolChemical(buoyancy, viscosity, health);
        }

        // Utility functions
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

        private static string CompressList(string[] items, char separator)
        {
            return items.Length > 0
                ? items.Aggregate("", (current, property) => current + $"{separator}{property}")[1..] : "";
        }

        private static string[] ExpandList(string items, char separator)
        {
            return items.Length > 0 ? items.Split(separator) : Array.Empty<string>();
        }
    }
}
