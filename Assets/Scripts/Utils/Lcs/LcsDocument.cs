using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public byte[] Binify()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(Version));
            for (var i = 0; i < LineLevels.Length; i++)
            {
                result.AddRange(Encoding.UTF8.GetBytes(LineLevels[i]));
                result.Add(i < LineLevels.Length - 1 ? (byte)1 : (byte)0);
            }
            foreach (var line in Lines)
            {
                result.AddRange(line.Binify());
            }
            return LcsInfo.GzipCompress(result.ToArray());
        }

        public static LcsDocument Parse(byte[] raw)
        {
            raw = LcsInfo.GzipDecompress(raw);
            var lines = new List<LcsLine>();
            var version = BitConverter.ToInt32(raw[..4]);
            var lineLevels = new List<string> { "" };
            var cursor = 4;
            while (raw[cursor] > 0)
            {
                if (raw[cursor] > 1)
                {
                    lineLevels[^1] += (char)raw[cursor];
                }
                else
                {
                    lineLevels.Add("");
                }
                cursor++;
            }
            cursor++;
            var lineLevelMap = new Dictionary<char, int>();
            for (var i = 0; i < lineLevels.Count; i++) foreach (var linePrefix in lineLevels[i])
            {
                lineLevelMap[linePrefix] = i;
            }
            var lineCache = new Dictionary<int, LcsLine>();
            while (cursor < raw.Length)
            {
                var line = LcsLine.Parse(raw, ref cursor);
                if (!lineLevelMap.ContainsKey(line.Prefix)) throw Errors.InvalidItem("LCS line prefix", line.Prefix);
                var lineLevel = lineLevelMap[line.Prefix];
                lineCache[lineLevel] = line;
                if (lineLevel == 0)
                {
                    lines.Add(line);
                }
                else
                {
                    lineCache[lineLevel - 1].Children.Add(line);
                }
                cursor++;
            }
            return new LcsDocument(version, lines, lineLevels.ToArray());
        }

        public string Stringify()
        {
            return Version.ToString() + LcsInfo.PropertySeparator + LcsInfo.ConcatProps(LineLevels)
                   + '\n' + Lines.Aggregate("", (current, line) => current + line.Stringify());
        }

        public static LcsDocument Parse(string raw)
        {
            var lines = new List<LcsLine>();
            var rawLines = raw.Trim().Split('\n');
            var headerParts = rawLines[0].Split(LcsInfo.PropertySeparator);
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
                if (!lineLevelMap.ContainsKey(line.Prefix)) throw Errors.InvalidItem("LCS line prefix", line.Prefix);
                var lineLevel = lineLevelMap[line.Prefix];
                lineCache[lineLevel] = line;
                if (lineLevel == 0)
                {
                    lines.Add(line);
                }
                else
                {
                    lineCache[lineLevel - 1].Children.Add(line);
                }
            }
            return new LcsDocument(version, lines, lineLevels);
        }

        public override string ToString()
        {
            return Lines.Aggregate("{{{\n", (current, line) => current + $"\t{line}\n") + "}}}";
        }
    }
}
