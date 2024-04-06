using System;
using System.Linq;
using System.Text.RegularExpressions;
using Utils.Ext;
using Utils.Object;

namespace Utils
{
    public static class BsUtils
    {
        public const char PropertySeperator = ';';

        public static int Layer2Order(ObjectLayer layer)
        {
            return layer switch
            {
                ObjectLayer.Background => -2,
                ObjectLayer.Foreground => 2,
                _ => 0
            };
        }

        public static float CalculateDamage(float impactForce)
        {
            return MathfExt.TMP(impactForce, 20f, .5f);
        }

        public static uint GenerateId(string title, string author)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes((title + author).GetHashCode()), 0);
        }

        public static string GenerateFullId(string tag, string id)
        {
            return $"{tag}:{id}";
        }

        public static string[] SplitFullId(string fullId)
        {
            return fullId.Split(':');
        }

        public static string CompressProperties(string[] properties)
        {
            return properties.Aggregate("", (current, property) => current + $"{PropertySeperator}{property}")[1..];
        }

        public static string[] ExpandProperties(string propetries)
        {
            return propetries.Split(PropertySeperator);
        }

        public static string CompressMapString(string value)
        {
            return YamlToLcs(value);
        }

        public static string ExpandMapString(string value)
        {
            return LcsToYaml(value);
        }

        public static string YamlToLcs(string yaml)
        {
            var result = yaml;
            result = Regex.Replace(result, @"tt: (.+)\nat: (.+)\nsz: (.+)\nlg: (.+)", "!$1;$2;$3;$4");
            result = Regex.Replace(result, @"- ps: (.+)\n  pr: (.+)", "$$$1;$2");
            result = Regex.Replace(result, @"- id: (.+)\n  pr: (.+)\n  ad:( \[\])?", "#$1;$2");
            result = Regex.Replace(result, @"  - id: (.+)\n    pr: (.+)", "@$1;$2");
            result = Regex.Replace(result, @"(sp|ob):\n", "");
            result = result.Replace(' ', ',');
            return result[..^1];
        }

        public static string LcsToYaml(string lcs)
        {
            var result = lcs + '\n';
            result = result.Replace(',', ' ');
            result = Regex.Replace(result, @"(?<=!.+)\n", "\nsp:\n");
            result = Regex.Replace(result, @"(?<=\$.+)\n(?=#)", "\nob:\n");
            result = Regex.Replace(result, @"@([^;]+);(.+)", "  - id: $1\n    pr: $2");
            result = Regex.Replace(result, @"#([^;]+);(.+)", "- id: $1\n  pr: $2\n  ad: []");
            result = Regex.Replace(result, @"(?<=  ad:) \[\](?=\n )", "");
            result = Regex.Replace(result, @"\$([^;]+);(.+)", "- ps: $1\n  pr: $2");
            result = Regex.Replace(result, @"!([^;]+);([^;]+);([^;]+);([^;]+)", "tt: $1\nat: $2\nsz: $3\nlg: $4");
            return result;
        }
    }
}
