using System;
using System.Linq;

namespace Utils.Lcs
{
    public static class LcsParser
    {
        public const char PropertySeperator = ';';
        public const char FieldSeperator = ',';

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

        public static string CompressList(string[] items, char separator)
        {
            return items.Length > 0
                ? items.Aggregate("", (current, property) => current + $"{separator}{property}")[1..] : "";
        }

        public static string[] ExpandList(string items, char separator)
        {
            return items.Length > 0 ? items.Split(separator) : Array.Empty<string>();
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
