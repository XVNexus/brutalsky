using System.Collections.Generic;
using System.Linq;
using Utils.Constants;

namespace Utils.Lcs
{
    public class LcsDocument
    {
        public int Version { get; set; }
        public List<LcsLine> Lines { get; set; }
        public string[] LineLevels { get; set; }

        public LcsDocument(int version, List<LcsLine> lines, string[] lineLevels)
        {
            Version = version;
            Lines = lines;
            LineLevels = lineLevels;
        }

        public static LcsDocument Parse(string raw)
        {
            var result = new List<LcsLine>();
            var rawLines = raw.Trim().Split('\n');
            var headerParts = rawLines[0].Split(Stringifier.PropertySeperator);
            var version = int.Parse(headerParts[0]);
            var lineLevels = headerParts[1..];
            var lineLevelMap = new Dictionary<char, int>();
            for (var i = 0; i < lineLevels.Length; i++) foreach (var linePrefix in lineLevels[i])
            {
                lineLevelMap[linePrefix] = i;
            }
            var lineCache = new Dictionary<int, LcsLine>();
            for (var i = 1; i < rawLines.Length; i++)
            {
                var rawLine = rawLines[i];
                var line = LcsLine.Parse(rawLine);
                if (!lineLevelMap.ContainsKey(line.Prefix))
                {
                    throw Errors.InvalidItem("LCS line prefix", line.Prefix);
                }
                var lineLevel = lineLevelMap[line.Prefix];
                lineCache[lineLevel] = line;
                if (lineLevel > 0)
                {
                    lineCache[lineLevel - 1].Children.Add(line);
                }
                else
                {
                    result.Add(line);
                }
            }
            return new LcsDocument(version, result.ToList(), lineLevels);
        }

        public string Stringify()
        {
            return Version.ToString() + Stringifier.PropertySeperator + Stringifier.CompressProperties(LineLevels)
                + '\n' + Lines.Aggregate("", (current, line) => current + line.Stringify());
        }
    }
}
