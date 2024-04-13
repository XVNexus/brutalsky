using System.Collections.Generic;
using System.Linq;
using Utils.Constants;

namespace Utils.Lcs
{
    public class LcsDocument
    {
        public List<LcsLine> Lines { get; set; }
        public string[] LineLevels { get; set; }

        public LcsDocument(List<LcsLine> lines, string[] lineLevels)
        {
            Lines = lines;
            LineLevels = lineLevels;
        }

        public static LcsDocument Parse(string raw)
        {
            var version = int.Parse(raw[..3]);
            var lcs = raw[4..];
            return version switch
            {
                1 => Parse001(lcs),
                _ => throw Errors.InvalidLcsVersion(version)
            };
        }

        public string Stringify()
        {
            var header = LcsParser.Version.ToString().PadLeft(3, '0') + '\n';
            var lcs = LcsParser.Version switch
            {
                1 => Stringify001(),
                _ => throw Errors.InvalidLcsVersion(LcsParser.Version)
            };
            return header + lcs;
        }

        private static LcsDocument Parse001(string raw)
        {
            var result = new List<LcsLine>();
            var rawLines = raw.Trim().Split('\n');
            var lineLevels = rawLines[0].Split(' ');
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
            return new LcsDocument(result.ToList(), lineLevels);
        }

        private string Stringify001()
        {
            var lineLevelsString = LineLevels.Aggregate("", (current, lineLevel) => current + ' ' + lineLevel)[1..];
            var linesString = Lines.Aggregate("", (current, line) => current + line.Stringify());
            return lineLevelsString + '\n' + linesString;
        }
    }
}
