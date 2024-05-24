using System;
using System.Collections.Generic;
using Brutalsky.Scripts.Lcs;

namespace Brutalsky.Scripts.Data;

public class BsObject : ILcsLine
{
    public int Type { get; set; }
    public string Id { get; set; } = "";
    public string[] References { get; set; } = Array.Empty<string>();
    public object[] Props { get; set; } = Array.Empty<object>();

    public BsObject(int type, string id, params string[] references)
    {
        Type = type;
        Id = id;
        References = references;
    }

    public BsObject() { }

    public LcsLine _ToLcs()
    {
        var result = new List<object> { Type, Id };
        result.Add(References.Length);
        result.AddRange(References);
        result.AddRange(Props);
        return new LcsLine('#', result);
    }

    public void _FromLcs(LcsLine line)
    {
        var i = 0;
        Type = (int)line.Props[i++];
        Id = (string)line.Props[i++];
        References = new string[(int)line.Props[i++]];
        while (i < References.Length + 3)
        {
            References[i - 3] = (string)line.Props[i++];
        }
        Props = new object[line.Props.Length - References.Length - 3];
        while (i < line.Props.Length)
        {
            Props[i - References.Length - 3] = line.Props[i];
        }
    }
}
