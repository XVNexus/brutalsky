using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lcs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    public class BsNode : ILcsLine
    {
        public string Tag { get; set; }
        [CanBeNull] public Action Init { get; set; }
        public Func<float[], float> Calc { get; set; }

        public BsNode(string tag, Action init, Func<float[], float> calc)
        {
            Tag = tag;
            Init = init;
            Calc = calc;
        }

        public BsNode(string tag, Func<float[], float> calc)
        {
            Tag = tag;
            Calc = calc;
        }

        public BsNode() { }

        public static BsNode RandomBool()
        {
            return new BsNode("rbl", _ => ToLogic(Random.Range(0, 2)));
        }

        public static BsNode RandomInt()
        {
            return new BsNode("rin", inputs => inputs.Length switch
            {
                0 => ToLogic(Random.Range(0, 2)),
                1 => ToLogic(Random.Range(0, ToInt(inputs[0]))),
                _ => ToLogic(Random.Range(ToInt(inputs[0]), ToInt(inputs[1])))
            });
        }

        public static BsNode RandomFloat()
        {
            return new BsNode("rfl", inputs => inputs.Length switch
            {
                0 => Random.value,
                1 => Random.Range(0f, inputs[0]),
                _ => Random.Range(inputs[0], inputs[1])
            });
        }

        public static BsNode Timer()
        {
            var startTime = 0f;
            return new BsNode("tmr", () => startTime = Time.timeSinceLevelLoad, _ =>
                Time.timeSinceLevelLoad - startTime);
        }

        public static BsNode Clock()
        {
            var state = false;
            var lastPulse = 0f;
            return new BsNode("clk", () => lastPulse = Time.timeSinceLevelLoad, inputs =>
            {
                if (inputs.Length == 0)
                {
                    state = !state;
                }
                else if (Time.timeSinceLevelLoad >= lastPulse + inputs[0])
                {
                    state = !state;
                    lastPulse += inputs[0];
                }
                return ToLogic(state);
            });
        }

        public static BsNode Delay()
        {
            var savedValues = new Queue<(float, float)>();
            var state = 0f;
            return new BsNode("dly", () => savedValues = new Queue<(float, float)>(), inputs =>
            {
                if (inputs.Length >= 2)
                {
                    savedValues.Enqueue((Time.timeSinceLevelLoad, inputs[0]));
                    while (Time.timeSinceLevelLoad > savedValues.Peek().Item1 + inputs[1])
                    {
                        state = savedValues.Dequeue().Item2;
                    }
                }
                else
                {
                    state = inputs[0];
                }
                return state;
            });
        }

        public static BsNode Multiplex()
        {
            return new BsNode("mux", inputs => inputs.Length switch
            {
                0 => 0f,
                1 => inputs[0],
                _ => inputs[Mathf.Clamp(ToInt(inputs[0]) + 1, 1, inputs.Length - 1)]
            });
        }

        public static BsNode Demultiplex()
        {
            return new BsNode("dmx", inputs => inputs.Length switch
            {
                0 => 0f,
                1 => inputs[0],
                2 => ToBool(inputs[0]) ? inputs[1] : 0f,
                _ => ToInt(inputs[0]) == ToInt(inputs[1]) ? inputs[2] : 0f
            });
        }

        public static BsNode Buffer()
        {
            return new BsNode("buf", inputs => inputs.Length >= 1 ? ToLogic(ToBool(inputs[0])) : 0f);
        }

        public static BsNode Not()
        {
            return new BsNode("not", inputs => inputs.Length >= 1 ? ToLogic(!ToBool(inputs[0])) : 0f);
        }

        public static BsNode And()
        {
            return new BsNode("and", inputs =>
                ToLogic(inputs.Aggregate(true, (current, input) => current & ToBool(input))));
        }

        public static BsNode Or()
        {
            return new BsNode("lor", inputs =>
                ToLogic(inputs.Aggregate(false, (current, input) => current | ToBool(input))));
        }

        public static BsNode Xor()
        {
            return new BsNode("xor", inputs =>
                ToLogic(inputs.Aggregate(false, (current, input) => current ^ ToBool(input))));
        }

        public static BsNode Nand()
        {
            return new BsNode("nan", inputs =>
                ToLogic(!inputs.Aggregate(true, (current, input) => current & ToBool(input))));
        }

        public static BsNode Nor()
        {
            return new BsNode("nor", inputs =>
                ToLogic(!inputs.Aggregate(false, (current, input) => current | ToBool(input))));
        }

        public static BsNode Xnor()
        {
            return new BsNode("xnr", inputs =>
                ToLogic(!inputs.Aggregate(false, (current, input) => current ^ ToBool(input))));
        }

        public static BsNode Add()
        {
            return new BsNode("add", inputs =>
                inputs.Aggregate(0f, (current, input) => current + input));
        }

        public static BsNode Subtract()
        {
            return new BsNode("sub", inputs => inputs.Length switch
            {
                0 => 0f,
                1 => -inputs[0],
                _ => inputs[1..].Aggregate(inputs[0], (current, input) => current - input)
            });
        }

        public static BsNode Multiply()
        {
            return new BsNode("mul", inputs =>
                inputs.Aggregate(1f, (current, input) => current * input));
        }

        public static BsNode Divide()
        {
            return new BsNode("div", inputs => inputs.Length switch
            {
                0 => 1f,
                1 => 1f / inputs[0],
                _ => inputs[1..].Aggregate(inputs[0], (current, input) => current / input)
            });
        }

        public static BsNode Power()
        {
            return new BsNode("pow", inputs => inputs.Length switch
            {
                0 => 1f,
                1 => Mathf.Pow(inputs[0], 2f),
                _ => Mathf.Pow(inputs[0], inputs[1])
            });
        }

        public static BsNode Root()
        {
            return new BsNode("rot", inputs => inputs.Length switch
            {
                0 => 1f,
                1 => Mathf.Sqrt(inputs[0]),
                _ => Mathf.Pow(inputs[0], 1f / inputs[1])
            });
        }

        public static BsNode Sin()
        {
            return new BsNode("sin", inputs => inputs.Length >= 1 ? Mathf.Sin(inputs[1]) : 0f);
        }

        public static BsNode Cos()
        {
            return new BsNode("cos", inputs => inputs.Length >= 1 ? Mathf.Cos(inputs[1]) : 0f);
        }

        public static BsNode Tan()
        {
            return new BsNode("tan", inputs => inputs.Length >= 1 ? Mathf.Tan(inputs[1]) : 0f);
        }

        public static BsNode Asin()
        {
            return new BsNode("asn", inputs => inputs.Length >= 1 ? Mathf.Asin(inputs[1]) : 0f);
        }

        public static BsNode Acos()
        {
            return new BsNode("acs", inputs => inputs.Length >= 1 ? Mathf.Acos(inputs[1]) : 0f);
        }

        public static BsNode Atan()
        {
            return new BsNode("atn", inputs => inputs.Length >= 1 ? Mathf.Atan(inputs[1]) : 0f);
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

        public LcsLine _ToLcs()
        {
            return new LcsLine('%', Tag);
        }

        public void _FromLcs(LcsLine line)
        {
            var result = line.Get<string>(0) switch
            {
                "rbl" => RandomBool(),
                "rin" => RandomInt(),
                "rfl" => RandomFloat(),
                "tmr" => Timer(),
                "clk" => Clock(),
                "dly" => Delay(),
                "mux" => Multiplex(),
                "dmx" => Demultiplex(),
                "buf" => Buffer(),
                "not" => Not(),
                "and" => And(),
                "lor" => Or(),
                "xor" => Xor(),
                "nan" => Nand(),
                "nor" => Nor(),
                "xnr" => Xnor(),
                "add" => Add(),
                "sub" => Subtract(),
                "mul" => Multiply(),
                "div" => Divide(),
                "pow" => Power(),
                "rot" => Root(),
                "sin" => Sin(),
                "cos" => Cos(),
                "tan" => Tan(),
                "asn" => Asin(),
                "acs" => Acos(),
                "atn" => Atan(),
                _ => RandomBool()
            };
            Tag = result.Tag;
            Init = result.Init;
            Calc = result.Calc;
        }
    }
}
