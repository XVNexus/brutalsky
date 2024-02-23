using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utils
{
    public static class YamlCompressor
    {
        public const string Base26Charset = "abcdefghijklmnopqrstuvwxyz";
        public static string AliasKeys(string yaml)
        {
            Debug.Log("baller");
            var aliasMap = new Dictionary<string, string>();
            var keysByUsage = new Dictionary<string, int>();
            foreach (Match keyMatch in Regex.Matches(yaml, @"\w+(?=:)"))
            {
                var key = keyMatch.Value;
                if (!keysByUsage.TryAdd(key, 1))
                {
                    keysByUsage[key]++;
                }
            }
            var sortedKeys = keysByUsage.Keys.ToList();
            sortedKeys.Sort((a, b) => keysByUsage[b] - keysByUsage[a]);
            for (var i = 0; i < sortedKeys.Count; i++)
            {
                var key = sortedKeys[i];
                var alias = Convert.ToString(i, 26);
            }

            return "";
        }

        public static string Num2Alpha(int num)
        {
            var baseConverter = new BaseConverter(Base26Charset);
            return baseConverter.Num2Base(num);
        }

        public static int Alpha2Num(string alpha)
        {
            var baseConverter = new BaseConverter(Base26Charset);
            return baseConverter.Base2Num(alpha);
        }
    }
}
