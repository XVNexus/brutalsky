using System;
using System.Linq;

namespace Utils.Lcs
{
    public static class LcsParser
    {
        public const char PropertySeperator = ';';
        public const char FieldSeperator = ',';

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
