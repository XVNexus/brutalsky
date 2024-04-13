using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utils.Constants;

namespace Utils.Lcs
{
    public class LcsDocument
    {
        public List<LcsLine> Lines { get; set; }

        public LcsDocument(List<LcsLine> lines)
        {
            Lines = lines;
        }

        public static LcsDocument Parse(string raw)
        {
            var version = int.Parse(raw[..3]);
            var lcs = raw[4..];
            return version switch
            {
                1 => Parse001(lcs),
                2 => Parse002(lcs),
                _ => throw Errors.InvalidLcsVersion(version)
            };
        }

        public string Stringify()
        {
            var header = LcsParser.Version.ToString().PadLeft(3, '0') + '\n';
            var lcs = LcsParser.Version switch
            {
                1 => Stringify001(),
                2 => Stringify002(),
                _ => throw Errors.InvalidLcsVersion(LcsParser.Version)
            };
            return header + lcs;
        }

        private static LcsDocument Parse002(string raw)
        {
            var result = raw;
            result = Regex.Replace(result, "[a-m]",
                match => BsUtils.RepeatChar(LcsParser.FieldSeperator, match.Value[0] - 96));
            result = Regex.Replace(result, "[n-z]",
                match => BsUtils.RepeatChar(LcsParser.PropertySeperator, match.Value[0] - 109));
            return Parse001(result);
        }

        private static LcsDocument Parse001(string raw)
        {
            var result = new List<LcsLine>();
            var rawLines = raw.Trim().Split('\n');
            var lineLevels = new Dictionary<char, int> { { '!', 0 }, { '$', 0 }, { '#', 0 }, { '@', 1 } };
            var lineCache = new Dictionary<int, LcsLine>();
            var lastLineLevel = 0;
            foreach (var rawLine in rawLines)
            {
                var line = LcsLine.Parse(rawLine);
                var lineLevel = lineLevels[line.Prefix];
                lineCache[lineLevel] = line;
                if (lineLevel > lastLineLevel)
                {
                    lineCache[lastLineLevel].Children.Add(line);
                }
                else
                {
                    result.Add(line);
                }
                lastLineLevel = lineLevel;
            }
            return new LcsDocument(result.ToList());
        }

        private string Stringify002()
        {
            var result = Stringify001();
            result = Regex.Replace(result, $"{LcsParser.FieldSeperator}{{1,13}}",
                match => $"{(char)(match.Length + 96)}");
            result = Regex.Replace(result, $"{LcsParser.PropertySeperator}{{1,13}}",
                match => $"{(char)(match.Length + 109)}");
            return result;
        }

        private string Stringify001()
        {
            return Lines.Aggregate("", (current, line) => current + line.Stringify());
        }
    }
}
