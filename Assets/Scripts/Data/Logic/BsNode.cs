using System;
using System.Linq;
using JetBrains.Annotations;
using Lcs;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Data.Logic
{
    public class BsNode : IHasId
    {
        public string Tag { get; set; }
        public string Id { get; set; }
        public object[] Config { get; }
        public float[] Inputs { get; set; }
        public float[] Outputs { get; private set; }
        public Func<(float[], float[])> Init { get; set; } = () => (Array.Empty<float>(), Array.Empty<float>());
        [CanBeNull] public Func<float[], float[]> Update { get; set; }

        public BsNode(string tag, string id, params object[] config)
        {
            Tag = tag;
            Id = id;
            Config = config;
        }

        public BsNode() { }

        public void Reset()
        {
            var defaultPorts = Init();
            Inputs = defaultPorts.Item1;
            Outputs = defaultPorts.Item2;
        }

        public void Step()
        {
            if (Update == null) return;
            var result = Update(Inputs);
            Outputs = result;
        }

        // Basic nodes
        public static BsNode ConstantBool(string id, bool value)
        {
            var logicValue = ToLogic(value);
            return new BsNode("bcbl", id, value)
            {
                Init = () => (Array.Empty<float>(), new[] { logicValue })
            };
        }

        public static BsNode ConstantInt(string id, int value)
        {
            var logicValue = ToLogic(value);
            return new BsNode("bcin", id, value)
            {
                Init = () => (Array.Empty<float>(), new[] { logicValue })
            };
        }

        public static BsNode ConstantFloat(string id, float value)
        {
            return new BsNode("bcfl", id, value)
            {
                Init = () => (Array.Empty<float>(), new[] { value })
            };
        }

        public static BsNode RandomBool(string id)
        {
            var state = 0f;
            return new BsNode("brbl", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs =>
                {
                    if (ToBool(inputs[0]))
                    {
                        state = ToLogic(Random.Range(0, 2));
                    }
                    return new[] { state };
                }
            };
        }

        public static BsNode RandomInt(string id, int min, int max)
        {
            var state = 0f;
            return new BsNode("brin", id, min, max)
            {
                Init = () => (new[] { 0f, ToLogic(min), ToLogic(max) }, new float[1]),
                Update = inputs =>
                {
                    if (ToBool(inputs[0]))
                    {
                        state = ToLogic(Random.Range(ToInt(inputs[1]), ToInt(inputs[2]) + 1));
                    }
                    return new[] { state };
                }
            };
        }

        public static BsNode RandomFloat(string id, float min, float max)
        {
            var state = 0f;
            return new BsNode("brfl", id, min, max)
            {
                Init = () => (new[] { 0f, min, max }, new float[1]),
                Update = inputs =>
                {
                    if (ToBool(inputs[0]))
                    {
                        state = Random.Range(inputs[1], inputs[2]);
                    }
                    return new[] { state };
                }
            };
        }

        // Flow nodes
        public static BsNode Timer(string id)
        {
            var startTime = 0f;
            return new BsNode("ftmr", id)
            {
                Init = () =>
                {
                    startTime = Time.timeSinceLevelLoad;
                    return (Array.Empty<float>(), Array.Empty<float>());
                },
                Update = _ => new[] { Time.timeSinceLevelLoad - startTime }
            };
        }

        public static BsNode Memory(string id)
        {
            var state = 0f;
            return new BsNode("fmem", id)
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

        public static BsNode Delay(string id, int time)
        {
            var state = new float[time];
            return new BsNode("fdly", id, time)
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

        public static BsNode Clock(string id, float interval, bool precise)
        {
            var state = false;
            var timer = 0f;
            var counter = 0;
            return new BsNode("fclk", id, interval, precise)
            {
                Init = () =>
                {
                    if (precise)
                    {
                        timer = Time.timeSinceLevelLoad;
                    }
                    else
                    {
                        counter = 0;
                    }
                    return (Array.Empty<float>(), new float[1]);
                },
                Update = precise
                    ? _ =>
                    {
                        if (Time.timeSinceLevelLoad - timer >= interval)
                        {
                            state = !state;
                            timer += interval;
                        }
                        return new[] { ToLogic(state) };
                    }
                    : _ =>
                    {
                        if (counter++ >= interval)
                        {
                            state = !state;
                            counter = 0;
                        }
                        return new[] { ToLogic(state) };
                    }
            };
        }

        public static BsNode Monostable(string id)
        {
            var lastInput = 0f;
            return new BsNode("fmst", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs =>
                {
                    if (Mathf.Approximately(inputs[0], lastInput)) return new[] { 0f };
                    lastInput = inputs[0];
                    return new[] { 1f };
                }
            };
        }

        public static BsNode Multiplexer(string id, int inputCount)
        {
            return new BsNode("fmux", id, inputCount)
            {
                Init = () => (new float[inputCount + 1], new float[1]),
                Update = inputs => new[] { inputs[ToInt(inputs[0]) + 1] }
            };
        }

        public static BsNode Demultiplexer(string id, int outputCount)
        {
            return new BsNode("fdmx", id, outputCount)
            {
                Init = () => (new float[2], new float[outputCount]),
                Update = inputs =>
                {
                    var result = new float[outputCount];
                    result[ToInt(inputs[0])] = inputs[1];
                    return result;
                }
            };
        }

        // Logic nodes
        public static BsNode Buffer(string id)
        {
            return new BsNode("lbuf", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { ToLogic(ToBool(inputs[0])) }
            };
        }

        public static BsNode Not(string id)
        {
            return new BsNode("lnot", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { ToLogic(!ToBool(inputs[0])) }
            };
        }

        public static BsNode And(string id, int inputCount)
        {
            return new BsNode("land", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs =>
                    new[] { ToLogic(inputs.Aggregate(true, (current, input) => current & ToBool(input))) }
            };
        }

        public static BsNode Or(string id, int inputCount)
        {
            return new BsNode("lgor", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs =>
                    new[] { ToLogic(inputs.Aggregate(true, (current, input) => current | ToBool(input))) }
            };
        }

        public static BsNode Xor(string id, int inputCount)
        {
            return new BsNode("lxor", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs =>
                    new[] { ToLogic(inputs.Aggregate(true, (current, input) => current ^ ToBool(input))) }
            };
        }

        public static BsNode Nand(string id, int inputCount)
        {
            return new BsNode("lnan", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs =>
                    new[] { ToLogic(!inputs.Aggregate(true, (current, input) => current & ToBool(input))) }
            };
        }

        public static BsNode Nor(string id, int inputCount)
        {
            return new BsNode("lnor", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs =>
                    new[] { ToLogic(!inputs.Aggregate(true, (current, input) => current | ToBool(input))) }
            };
        }

        public static BsNode Xnor(string id, int inputCount)
        {
            return new BsNode("lxnr", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs =>
                    new[] { ToLogic(!inputs.Aggregate(true, (current, input) => current ^ ToBool(input))) }
            };
        }

        // Math nodes
        public static BsNode Add(string id, int inputCount)
        {
            return new BsNode("madd", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs => new[] { inputs.Aggregate(0f, (current, input) => current + input) }
            };
        }

        public static BsNode Subtract(string id, int inputCount)
        {
            return new BsNode("msub", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputCount > 1
                    ? inputs => new[] { inputs[1..].Aggregate(inputs[0], (current, input) => current - input) }
                    : inputCount == 1
                        ? inputs => new[] { 0f - inputs[0] }
                        : _ => new[] { 0f }
            };
        }

        public static BsNode Multiply(string id, int inputCount)
        {
            return new BsNode("mmul", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputs => new[] { inputs.Aggregate(1f, (current, input) => current * input) }
            };
        }

        public static BsNode Divide(string id, int inputCount)
        {
            return new BsNode("mdiv", id, inputCount)
            {
                Init = () => (new float[inputCount], new float[1]),
                Update = inputCount > 1
                    ? inputs => new[] { inputs[1..].Aggregate(inputs[0], (current, input) => current / input) }
                    : inputCount == 1
                        ? inputs => new[] { 1f / inputs[0] }
                        : _ => new[] { 1f }
            };
        }

        public static BsNode Pow(string id)
        {
            return new BsNode("mpow", id)
            {
                Init = () => (new float[2], new float[1]),
                Update = inputs => new[] { Mathf.Pow(inputs[0], inputs[1]) }
            };
        }

        public static BsNode Root(string id)
        {
            return new BsNode("mrot", id)
            {
                Init = () => (new float[2], new float[1]),
                Update = inputs => new[] { Mathf.Pow(inputs[0], 1f / inputs[1]) }
            };
        }

        public static BsNode Sin(string id)
        {
            return new BsNode("msin", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { Mathf.Sin(inputs[0]) }
            };
        }

        public static BsNode Cos(string id)
        {
            return new BsNode("mcos", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { Mathf.Cos(inputs[0]) }
            };
        }

        public static BsNode Tan(string id)
        {
            return new BsNode("mtan", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { Mathf.Tan(inputs[0]) }
            };
        }

        public static BsNode Asin(string id)
        {
            return new BsNode("masn", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { Mathf.Asin(inputs[0]) }
            };
        }

        public static BsNode Acos(string id)
        {
            return new BsNode("macs", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { Mathf.Acos(inputs[0]) }
            };
        }

        public static BsNode Atan(string id)
        {
            return new BsNode("matn", id)
            {
                Init = () => (new float[1], new float[1]),
                Update = inputs => new[] { Mathf.Atan(inputs[0]) }
            };
        }

        public static float ToLogic(bool value)
        {
            return value ? 1f : 0f;
        }

        public static bool ToBool(float logic)
        {
            return logic >= .5f;
        }

        public static float ToLogic(int value)
        {
            return value;
        }

        public static int ToInt(float logic)
        {
            return Mathf.RoundToInt(logic);
        }
    }
}
