using System;
using System.Collections.Generic;
using System.Linq;
using Brutalsky.Scripts.Extensions;
using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsLogicNode : ILcsLine
{
    // The identifier for this node type
    // Each type of node is assigned a unique tag for serialization purposes
    public string Tag { get; set; } = "";
    // The node metadata
    // This will be serialized in map files
    // Use this to save config options for the node that should persist across map and game reloads
    public object[] Config { get; set; } = Array.Empty<object>();
    // Called when the node is newly loaded
    // Use this function to set default inputs and outputs and/or reset saved states
    public Func<(float[], float[])> Init { get; set; } = () => (Array.Empty<float>(), Array.Empty<float>());
    // Called for every _PhysicsProcess while this node is loaded
    // Use this function to recalculate the output values based on the given input values
    public Func<float[], float[]>? Update { get; set; }

    public BsLogicNode(string tag, params object[] config)
    {
        Tag = tag;
        Config = config;
    }

    public BsLogicNode(string tag)
    {
        Tag = tag;
    }

    public BsLogicNode() { }

    // Basic nodes
    public static BsLogicNode ConstantBool(bool value)
    {
        return new BsLogicNode("bcbl", value)
        {
            Init = () => (Array.Empty<float>(), new[] { ToLogic(value) })
        };
    }

    public static BsLogicNode ConstantInt(int value)
    {
        return new BsLogicNode("bcin", value)
        {
            Init = () => (Array.Empty<float>(), new[] { ToLogic(value) })
        };
    }

    public static BsLogicNode ConstantFloat(float value)
    {
        return new BsLogicNode("bcfl", value)
        {
            Init = () => (Array.Empty<float>(), new[] { value })
        };
    }

    public static BsLogicNode RandomBool()
    {
        var random = new Random();
        var state = false;
        return new BsLogicNode("brbl")
        {
            Init = () => (new float[1], new[] { ToLogic(random.NextBoolean()) }),
            Update = inputs =>
            {
                if (ToBool(inputs[0]))
                {
                    state = random.NextBoolean();
                }
                return new[] { ToLogic(state) };
            }
        };
    }

    public static BsLogicNode RandomInt(int min, int max)
    {
        var random = new Random();
        var state = 0;
        return new BsLogicNode("brin", min, max)
        {
            Init = () => (new[] { 0f, ToLogic(min), ToLogic(max) }, new[] { ToLogic(random.NextInt32(min, max)) }),
            Update = inputs =>
            {
                if (ToBool(inputs[0]))
                {
                    state = random.NextInt32(ToInt(inputs[1]), ToInt(inputs[2]));
                }
                return new[] { ToLogic(state) };
            }
        };
    }

    public static BsLogicNode RandomFloat(float min, float max)
    {
        var random = new Random();
        var state = 0f;
        return new BsLogicNode("brfl", min, max)
        {
            Init = () => (new[] { 0f, min, max }, new[] { random.NextSingle(min, max) }),
            Update = inputs =>
            {
                if (ToBool(inputs[0]))
                {
                    state = random.NextSingle(inputs[1], inputs[2]);
                }
                return new[] { state };
            }
        };
    }

    // Flow nodes
    public static BsLogicNode Timer()
    {
        var startTime = 0uL;
        return new BsLogicNode("ftmr")
        {
            Init = () =>
            {
                startTime = Time.GetTicksMsec();
                return (Array.Empty<float>(), new float[1]);
            },
            Update = _ => new[] { (Time.GetTicksMsec() - startTime) / 1000f }
        };
    }

    public static BsLogicNode Memory()
    {
        var state = 0f;
        return new BsLogicNode("fmem")
        {
            Init = () => (new float[2], new float[1]),
            Update = inputs =>
            {
                if (ToBool(inputs[1]))
                {
                    state = inputs[0];
                }
                return new[] { state };
            }
        };
    }

    public static BsLogicNode Delay(int time)
    {
        var state = new float[time];
        return new BsLogicNode("fdly", time)
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs =>
            {
                for (var i = time - 1; i >= 1; i--)
                {
                    state[i] = state[i - 1];
                }
                state[0] = inputs[0];
                return new[] { state[^1] };
            }
        };
    }

    public static BsLogicNode Clock(int interval)
    {
        var state = false;
        var timer = 0;
        return new BsLogicNode("fclk", interval)
        {
            Init = () => (Array.Empty<float>(), new float[1]),
            Update = _ =>
            {
                if (timer++ >= interval)
                {
                    state = !state;
                    timer = 0;
                }
                return new[] { ToLogic(state) };
            }
        };
    }

    public static BsLogicNode Monostable()
    {
        var lastInput = 0f;
        return new BsLogicNode("fmst")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs =>
            {
                if (Math.Abs(inputs[0] - lastInput) < MathfExt.EqualityThreshold) return new[] { 0f };
                lastInput = inputs[0];
                return new[] { 1f };
            }
        };
    }

    public static BsLogicNode Multiplexer(int size)
    {
        return new BsLogicNode("fmux", size)
        {
            Init = () => (new float[size + 1], new float[1]),
            Update = inputs => new[] { inputs[ToInt(inputs[0]) + 1] }
        };
    }

    public static BsLogicNode Demultiplexer(int size)
    {
        return new BsLogicNode("fdmx", size)
        {
            Init = () => (new float[2], new float[size]),
            Update = inputs =>
            {
                var result = new float[size];
                result[ToInt(inputs[0])] = inputs[1];
                return result;
            }
        };
    }

    // Logic nodes
    public static BsLogicNode Buffer()
    {
        return new BsLogicNode("lbuf")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { ToLogic(ToBool(inputs[0])) }
        };
    }

    public static BsLogicNode Not()
    {
        return new BsLogicNode("lnot")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { ToLogic(!ToBool(inputs[0])) }
        };
    }

    public static BsLogicNode And(int size)
    {
        return new BsLogicNode("land", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { ToLogic(inputs.Aggregate(true, (current, input) => current && ToBool(input))) }
        };
    }

    public static BsLogicNode Or(int size)
    {
        return new BsLogicNode("lgor", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { ToLogic(inputs.Aggregate(false, (current, input) => current || ToBool(input))) }
        };
    }

    public static BsLogicNode Xor(int size)
    {
        return new BsLogicNode("lxor", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { ToLogic(inputs.Aggregate(false, (current, input) => current ^ ToBool(input))) }
        };
    }

    public static BsLogicNode Nand(int size)
    {
        return new BsLogicNode("lnan", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { ToLogic(!inputs.Aggregate(true, (current, input) => current && ToBool(input))) }
        };
    }

    public static BsLogicNode Nor(int size)
    {
        return new BsLogicNode("lnor", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { ToLogic(!inputs.Aggregate(false, (current, input) => current || ToBool(input))) }
        };
    }

    public static BsLogicNode Xnor(int size)
    {
        return new BsLogicNode("lxnr", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { ToLogic(!inputs.Aggregate(false, (current, input) => current ^ ToBool(input))) }
        };
    }

    // Math nodes
    public static BsLogicNode Increment()
    {
        return new BsLogicNode("minc")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { inputs[0] + 1f }
        };
    }

    public static BsLogicNode Decrement()
    {
        return new BsLogicNode("mdec")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { inputs[0] - 1f }
        };
    }

    public static BsLogicNode Add(int size)
    {
        return new BsLogicNode("madd", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { inputs.Sum() }
        };
    }

    public static BsLogicNode Subtract(int size)
    {
        return new BsLogicNode("msub", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = size > 1
                ? inputs => new[] { inputs[1..].Aggregate(inputs[0], (current, input) => current - input) }
                : size == 1
                    ? inputs => new[] { -inputs[0] }
                    : _ => new[] { 0f }
        };
    }

    public static BsLogicNode Multiply(int size)
    {
        return new BsLogicNode("mmul", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = inputs => new[] { inputs.Aggregate(1f, (current, input) => current * input) }
        };
    }

    public static BsLogicNode Divide(int size)
    {
        return new BsLogicNode("mdiv", size)
        {
            Init = () => (new float[size], new float[1]),
            Update = size > 1
                ? inputs => new[] { inputs[1..].Aggregate(inputs[0], (current, input) => current / input) }
                : size == 1
                    ? inputs => new[] { 1f / inputs[0] }
                    : _ => new[] { 1f }
        };
    }

    public static BsLogicNode Power()
    {
        return new BsLogicNode("mpow")
        {
            Init = () => (new float[2], new float[1]),
            Update = inputs => new[] { Mathf.Pow(inputs[0], inputs[1]) }
        };
    }

    public static BsLogicNode Root()
    {
        return new BsLogicNode("mrot")
        {
            Init = () => (new float[2], new float[1]),
            Update = inputs => new[] { Mathf.Pow(inputs[0], 1f / inputs[1]) }
        };
    }

    public static BsLogicNode Sin()
    {
        return new BsLogicNode("msin")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { Mathf.Sin(inputs[0]) }
        };
    }

    public static BsLogicNode Cos()
    {
        return new BsLogicNode("mcos")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { Mathf.Cos(inputs[0]) }
        };
    }

    public static BsLogicNode Tan()
    {
        return new BsLogicNode("mtan")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { Mathf.Tan(inputs[0]) }
        };
    }

    public static BsLogicNode Asin()
    {
        return new BsLogicNode("masn")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { Mathf.Asin(inputs[0]) }
        };
    }

    public static BsLogicNode Acos()
    {
        return new BsLogicNode("macs")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { Mathf.Acos(inputs[0]) }
        };
    }

    public static BsLogicNode Atan()
    {
        return new BsLogicNode("matn")
        {
            Init = () => (new float[1], new float[1]),
            Update = inputs => new[] { Mathf.Atan(inputs[0]) }
        };
    }

    public LcsLine _ToLcs()
    {
        var result = new List<object> { Tag };
        result.AddRange(Config);
        return new LcsLine('%', result);
    }

    public void _FromLcs(LcsLine line)
    {
        var i = 0;
        Tag = (string)line.Props[i++];
        Config = new object[line.Props.Length - 1];
        while (i < line.Props.Length)
        {
            Config[i - 1] = line.Props[i];
        }
        var result = Tag switch
        {
            "bcbl" => ConstantBool((bool)Config[0]),
            "bcin" => ConstantInt((int)Config[0]),
            "bcfl" => ConstantFloat((float)Config[0]),
            "brbl" => RandomBool(),
            "brin" => RandomInt((int)Config[0], (int)Config[1]),
            "brfl" => RandomFloat((float)Config[0], (float)Config[1]),
            "ftmr" => Timer(),
            "fmem" => Memory(),
            "fdly" => Delay((int)Config[0]),
            "fclk" => Clock((int)Config[0]),
            "fmst" => Monostable(),
            "fmux" => Multiplexer((int)Config[0]),
            "fdmx" => Demultiplexer((int)Config[0]),
            "lbuf" => Buffer(),
            "lnot" => Not(),
            "land" => And((int)Config[0]),
            "lgor" => Or((int)Config[0]),
            "lxor" => Xor((int)Config[0]),
            "lnan" => Nand((int)Config[0]),
            "lnor" => Nor((int)Config[0]),
            "lxnr" => Xnor((int)Config[0]),
            "minc" => Increment(),
            "mdec" => Decrement(),
            "madd" => Add((int)Config[0]),
            "msub" => Subtract((int)Config[0]),
            "mmul" => Multiply((int)Config[0]),
            "mdiv" => Divide((int)Config[0]),
            "mpow" => Power(),
            "mrot" => Root(),
            "msin" => Sin(),
            "mcos" => Cos(),
            "mtan" => Tan(),
            "masn" => Asin(),
            "macs" => Acos(),
            "matn" => Atan(),
            _ => throw Errors.InvalidItem("logic node tag", "tag")
        };
        Init = result.Init;
        Update = result.Update;
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
