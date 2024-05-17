using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Lcs;

public class Isolated : MonoBehaviour
{
    private void Start()
    {
        var samples = new[]
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor ",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor i",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
        };
        var lengths = new List<int>();
        for (var i = 0; i < 32; i++)
        {
            var baseNum = (int)Mathf.Pow(2, i);
            for (var j = -2; j <= 2; j++)
            {
                var num = baseNum + j;
                if (lengths.Contains(num) || num < 0) continue;
                lengths.Add(baseNum + j);
            }
        }
        lengths.Add(int.MaxValue - 1);
        lengths.Add(int.MaxValue);
        TestBinification(samples);
    }

    private void TestLength(IEnumerable<int> samples)
    {
        foreach (var sample in samples)
        {
            var bytes = IntToBytes(sample);
            var decoded = BytesToInt(bytes);
            Debug.Log($"{sample} ==> {PrintBytes(bytes)} ==> {decoded}  ||  {(sample == decoded ? ' ' : 'X')}");
            Debug.Log($"");
        }
    }

    private byte[] IntToBytes(int value)
    {
        var result = new List<byte>();
        var current = value;
        while (current > 127)
        {
            result.Add((byte)(current | 0x80));
            current /= 128;
        }
        result.Add((byte)current);
        return result.ToArray();
    }

    private int BytesToInt(byte[] value)
    {
        var result = 0;
        var cursor = 0;
        var power = 1;
        while ((value[cursor] & 0x80) > 0)
        {
            result += (value[cursor] & 0x7F) * power; 
            cursor++;
            power *= 128;
        }
        result += value[cursor] * power;
        return result;
    }

    private void TestBinification(IEnumerable<string> samples)
    {
        foreach (var sample in samples)
        {
            var prop = new LcsProp(sample);
            Debug.Log($"Sample length: {((string)prop.Value).Length}");
            var stringified = prop.Stringify();
            var destringified = LcsProp.Parse(stringified);
            var binified = prop.Binify();
            var cursor = 0;
            var debinified = LcsProp.Parse(binified, ref cursor);
            Debug.Log($"Original: {prop}");
            Debug.Log($"Str/destr: {destringified}");
            Debug.Log($"Bin/debin: {debinified}");
        }
    }

    private string PrintBytes(IEnumerable<byte> bytes)
    {
        return bytes.Aggregate("", (current, item) =>
            current + (Convert.ToString(item, 2).PadLeft(8, '0') + ' '));
    }
}
