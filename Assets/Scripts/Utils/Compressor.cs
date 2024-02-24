using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class Compressor
    {
        public const string BaseCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Compress(string mapString)
        {
            return Alias(mapString);
        }

        public static string Decompress(string mapString)
        {
            return Dealias(mapString);
        }

        public static string Alias(string text)
        {
            // Count how many times each key is repeated
            var unitsByUsage = new Dictionary<string, int>();
            foreach (Match unitMatch in Regex.Matches(text, @"\w+(?=:)"))
            {
                var unit = unitMatch.Value;
                if (!unitsByUsage.TryAdd(unit, 1))
                {
                    unitsByUsage[unit]++;
                }
            }

            // Sort all repeated units from most to least occurrences and generate shorthand aliases
            var sortedUnits = unitsByUsage.Keys.ToList();
            sortedUnits.Sort((a, b) => unitsByUsage[b] - unitsByUsage[a]);
            var aliasMap = new Dictionary<string, string>();
            for (var i = 0; i < sortedUnits.Count; i++)
            {
                var unit = sortedUnits[i];
                if (unitsByUsage[unit] == 1) break;
                aliasMap[unit] = Num2Alpha(i);
            }

            // Replace all repeated units with their aliases and append an alias map to the text
            var aliasMapString = aliasMap.Keys.Aggregate("", (current, key) => current + $"{key} ").TrimEnd();
            var result = Regex.Replace(text, @"\w+(?=:)", match
                => aliasMap.ContainsKey(match.Value) ? aliasMap[match.Value] : match.Value);
            return $"{aliasMapString}\n{result}";
        }

        public static string Dealias(string text)
        {
            // Extract the alias map from the first line of the file
            var aliasMap = new Dictionary<string, string>();
            var aliasListString = Regex.Match(text, @".+").Value;
            var aliasList = aliasListString.Split(' ');
            var cleanText = text[(aliasListString.Length + 1)..];
            for (var i = 0; i < aliasList.Length; i++)
            {
                aliasMap[Num2Alpha(i)] = aliasList[i];
            }

            // Replace all aliases with their full form
            return Regex.Replace(text, @"[A-Z]+(?=:)", match
                => aliasMap.ContainsKey(match.Value) ? aliasMap[match.Value] : match.Value);
        }

        public static string Num2Alpha(int num)
        {
            var baseConverter = new BaseConverter(BaseCharset);
            return baseConverter.Num2Base(num);
        }

        public static int Alpha2Num(string alpha)
        {
            var baseConverter = new BaseConverter(BaseCharset);
            return baseConverter.Base2Num(alpha);
        }
    }
}
