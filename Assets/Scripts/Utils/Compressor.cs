using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class Compressor
    {
        public const string BaseCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string CompressMap(string mapString)
        {
            return AliasMap(mapString, @"\S+");
        }

        public static string AliasMap(string text, string unitRegex)
        {
            // Count how many times each unit is repeated
            var unitsByUsage = new Dictionary<string, int>();
            foreach (Match unitMatch in Regex.Matches(text, unitRegex))
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
                if (unit.Length < 4) continue;
                if (unitsByUsage[unit] == 1) break;
                aliasMap[unit] = Num2Alpha(i);
            }

            // Replace all repeated units with their aliases and append an alias map to the text
            var aliasMapString = aliasMap.Keys.Aggregate("", (current, key) => current + $"{key} ").TrimEnd();
            var result = Regex.Replace(text, unitRegex, match
                => aliasMap.ContainsKey(match.Value) ? $"({aliasMap[match.Value]})" : match.Value);
            return $"{aliasMapString}\n{result}";
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
