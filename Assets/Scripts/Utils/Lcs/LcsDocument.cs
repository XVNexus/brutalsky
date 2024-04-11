using System.Collections.Generic;
using System.Linq;

namespace Utils.Lcs
{
    public class LcsDocument
    {
        public List<LcsLine> Lines { get; set; }

        public LcsDocument(List<LcsLine> lines)
        {
            Lines = lines;
        }

        public static LcsDocument Parse(string raw, Dictionary<char, int> lineLevels)
        {
            var result = new List<LcsLine>();
            var rawLines = raw.Trim().Split('\n');
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

        public string Stringify()
        {
            return Lines.Aggregate("", (current, line) => current + line.Stringify());
        }
    }
}
