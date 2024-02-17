using System.Linq;

namespace Utils
{
    public class Sanitizer
    {
        public static string Sanitize(string text)
        {
            return text
                .Replace("\\", @"\\")
                .Replace(" ", @"\s")
                .Replace("\t", @"\t")
                .Replace("\n", @"\n")
                .Replace("/", @"\d");
        }

        public static string Desanitize(string text)
        {
            return text
                .Replace(@"\d", "/")
                .Replace(@"\n", "\n")
                .Replace(@"\t", "\t")
                .Replace(@"\s", " ")
                .Replace(@"\\", "\\");
        }

        public static string Array2String(string[] array)
        {
            return array.Aggregate("", (current, item) => current + $"{item} / ")[..^3];
        }

        public static string[] String2Array(string text)
        {
            return text.Split(" / ");
        }
    }
}
