using System;
using System.Collections.Generic;
using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsNode : ILcsLine, IHasId
{
    public string Tag { get; set; } = "";
    public string Id { get; set; } = "";
    public object[] Config { get; set; } = Array.Empty<object>();

    public BsNode(string tag, string id, params object[] config)
    {
        Tag = tag;
        Id = id;
        Config = config;
    }

    public BsNode(string tag, string id)
    {
        Tag = tag;
        Id = id;
    }

    public BsNode() { }

    // Basic nodes
    public static BsNode ConstantBool(string id, bool value) => new("bcbl", id, value);

    public static BsNode ConstantInt(string id, int value) => new("bcin", id, value);

    public static BsNode ConstantFloat(string id, float value) => new("bcfl", id, value);

    public static BsNode RandomBool(string id) => new("brbl", id);

    public static BsNode RandomInt(string id, int min, int max) => new("brin", id, min, max);

    public static BsNode RandomFloat(string id, float min, float max) => new("brfl", id, min, max);

    // Flow nodes
    public static BsNode Timer(string id) => new("ftmr", id);

    public static BsNode Memory(string id) => new("fmem", id);

    public static BsNode Delay(string id, int time) => new("fdly", id, time);

    public static BsNode Clock(string id, int interval) => new("fclk", id, interval);

    public static BsNode Monostable(string id) => new("fmst", id);

    public static BsNode Multiplexer(string id, int size) => new("fmux", id, size);

    public static BsNode Demultiplexer(string id, int size) => new("fdmx", id, size);

    // Logic nodes
    public static BsNode Buffer(string id) => new("lbuf", id);

    public static BsNode Not(string id) => new("lnot", id);

    public static BsNode And(string id, int size) => new("land", id, size);

    public static BsNode Or(string id, int size) => new("lgor", id, size);

    public static BsNode Xor(string id, int size) => new("lxor", id, size);

    public static BsNode Nand(string id, int size) => new("lnan", id, size);

    public static BsNode Nor(string id, int size) => new("lnor", id, size);

    public static BsNode Xnor(string id, int size) => new("lxnr", id, size);

    // Math nodes
    public static BsNode Increment(string id) => new("minc", id);

    public static BsNode Decrement(string id) => new("mdec", id);

    public static BsNode Add(string id, int size) => new("madd", id, size);

    public static BsNode Subtract(string id, int size) => new("msub", id, size);

    public static BsNode Multiply(string id, int size) => new("mmul", id, size);

    public static BsNode Divide(string id, int size) => new("mdiv", id, size);

    public static BsNode Power(string id) => new("mpow", id);

    public static BsNode Root(string id) => new("mrot", id);

    public static BsNode Sin(string id) => new("msin", id);

    public static BsNode Cos(string id) => new("mcos", id);

    public static BsNode Tan(string id) => new("mtan", id);

    public static BsNode Asin(string id) => new("masn", id);

    public static BsNode Acos(string id) => new("macs", id);

    public static BsNode Atan(string id) => new("matn", id);

    public LcsLine _ToLcs()
    {
        var result = new List<object> { Tag, Id };
        result.AddRange(Config);
        return new LcsLine('%', result);
    }

    public void _FromLcs(LcsLine line)
    {
        var i = 0;
        Tag = (string)line.Props[i++];
        Id = (string)line.Props[i++];
        Config = new object[line.Props.Length - 1];
        while (i < line.Props.Length)
        {
            Config[i - 1] = line.Props[i];
        }
    }

    public static float ToLogic(bool value)
    {
        return value ? 1f : 0f;
    }

    public static float ToLogic(int value)
    {
        return value;
    }

    public static bool ToBool(float logic)
    {
        return logic >= .5f;
    }

    public static int ToInt(float logic)
    {
        return Mathf.RoundToInt(logic);
    }
}
