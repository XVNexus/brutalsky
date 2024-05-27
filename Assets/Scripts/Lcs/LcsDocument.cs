using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Utils.Constants;

namespace Lcs
{
    public struct LcsDocument
    {
        public const byte FormatString = 0;
        public const byte FormatBinary = 1;
        public const byte FormatGzip = 2;

        public int Version { get; }
        public string[] LineLevels { get; }
        public LcsLine[] Lines { get; }

        public LcsDocument(int version, string[] lineLevels, params LcsLine[] lines)
        {
            Version = version;
            LineLevels = lineLevels;
            Lines = lines;
        }

        public static LcsDocument Serialize<T>(T value) where T : ILcsDocument, new()
        {
            return value._ToLcs();
        }

        public static T Deserialize<T>(LcsDocument line) where T : ILcsDocument, new()
        {
            var result = new T();
            result._FromLcs(line);
            return result;
        }

        public byte[] Binify(bool useGzip = false)
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
            return useGzip ? GzipCompress(result.ToArray()) : result.ToArray();
        }

        public static LcsDocument Parse(byte[] raw, bool useGzip = false)
        {
            raw = useGzip ? GzipDecompress(raw) : raw;
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
            return new LcsDocument(version, lineLevels.ToArray(), lines.ToArray());
        }

        public string Stringify()
        {
            return Version + " " + string.Join(" ", LineLevels) + '\n' +
                   Lines.Aggregate("", (current, line) => current + line.Stringify());
        }

        public static LcsDocument Parse(string raw)
        {
            var lines = new List<LcsLine>();
            var rawLines = raw.Trim().Split('\n');
            var headerParts = rawLines[0].Split(' ');
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
                if (rawLine.Length == 0) continue;
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
            return new LcsDocument(version, lineLevels, lines.ToArray());
        }

        public static byte[] GzipCompress(byte[] source)
        {
            using var memoryStream = new MemoryStream();
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gzipStream.Write(source, 0, source.Length);
            }
            return memoryStream.ToArray();
        }

        public static byte[] GzipDecompress(byte[] source)
        {
            using var memoryStream = new MemoryStream(source);
            using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
            using var decompressedStream = new MemoryStream();
            gzipStream.CopyTo(decompressedStream);
            return decompressedStream.ToArray();
        }
    }
}
