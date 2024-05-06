/*
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Core;
*/
using UnityEngine;

public class Isolated : MonoBehaviour
{
    /*
    private void Start()
    {
        var sample = MapSystem.LoadMapAsset("Brutalsky").ToLcs().Binify();
        var compressed = Compress(sample);
        var decompressed = Decompress(compressed);
        Debug.Log($"full recovery: {CompareBytes(sample, decompressed)}");
        Debug.Log($"compression ratio: {Mathf.RoundToInt(100f - compressed.Length / (float)sample.Length * 100f)}%");
        Debug.Log($"sample length: {sample.Length}");
        Debug.Log($"compressed length: {compressed.Length}");
        Debug.Log($"compressed length: {decompressed.Length}");
    }

    public static byte[] Compress(byte[] source)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gzipStream.Write(source, 0, source.Length);
        }
        return memoryStream.ToArray();
    }

    public static byte[] Decompress(byte[] source)
    {
        using var memoryStream = new MemoryStream(source);
        using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        gzipStream.CopyTo(decompressedStream);
        return decompressedStream.ToArray();
    }

    public static bool CompareBytes(byte[] bytes1, byte[] bytes2)
    {
        return bytes1.Length == bytes2.Length && !bytes1.Where((t, i) => t != bytes2[i]).Any();
    }

    public static void PrintBytes(byte[] bytes)
    {
        Debug.Log(BitConverter.ToString(bytes).Replace('-', ' '));
    }

    public static void PrintSpans(IEnumerable<(byte, int)> spans)
    {
        Debug.Log(spans.Aggregate("", (current, span) =>
            current + $"{BitConverter.ToString(new[] { span.Item1 })}*{span.Item2} ")[..^1]);
    }
    */
}
