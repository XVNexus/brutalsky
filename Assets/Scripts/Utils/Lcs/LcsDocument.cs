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

        public static LcsDocument Parse(string raw)
        {
            var rawLines = raw.Trim().Split('\n');
            var lines = new LcsLine[rawLines.Length];
            for (var i = 0; i < rawLines.Length; i++)
            {
                lines[i] = LcsLine.Parse(rawLines[i]);
            }
            return new LcsDocument(lines.ToList());
        }

        public string Stringify()
        {
            return Lines.Aggregate("", (current, line) => current + (line.Stringify() + '\n'))[..^1];
        }
    }
}
