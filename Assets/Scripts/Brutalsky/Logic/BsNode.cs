using System;
using System.Linq;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Random = UnityEngine.Random;

namespace Brutalsky.Logic
{
    public class BsNode
    {
        public string Tag { get; set; }
        public float[] Config { get; }
        public float[] Inputs { get; set; }
        public float[] Outputs { get; private set; }
        private float[] _state = Array.Empty<float>();
        private readonly float[] _defaultInputs;
        private readonly float[] _defaultOutputs;
        private readonly Func<float[]> _init;
        private readonly Func<float[], float[], float[]> _update;

        public BsNode(string tag, float[] defaultInputs, float[] defaultOutputs,
            Func<float[]> init, Func<float[], float[], float[]> update, params float[] config)
        {
            Tag = tag;
            _defaultInputs = defaultInputs;
            _defaultOutputs = defaultOutputs;
            _init = init;
            _update = update;
            Config = config;
        }

        public BsNode(float[] defaultInputs, float[] defaultOutputs,
            Func<float[]> init, Func<float[], float[], float[]> update, params float[] config)
        {
            _defaultInputs = defaultInputs;
            _defaultOutputs = defaultOutputs;
            _init = init;
            _update = update;
            Config = config;
        }

        public BsNode(string tag, float[] defaultInputs, float[] defaultOutputs,
            Func<float[], float[], float[]> update, params float[] config)
        {
            Tag = tag;
            _defaultInputs = defaultInputs;
            _defaultOutputs = defaultOutputs;
            _update = update;
            Config = config;
        }

        public BsNode(float[] defaultInputs, float[] defaultOutputs,
            Func<float[], float[], float[]> update, params float[] config)
        {
            _defaultInputs = defaultInputs;
            _defaultOutputs = defaultOutputs;
            _update = update;
            Config = config;
        }

        public BsNode()
        {
        }

        public void Init()
        {
            Inputs = new float[_defaultInputs.Length];
            for (var i = 0; i < _defaultInputs.Length; i++)
            {
                Inputs[i] = _defaultInputs[i];
            }
            Outputs = new float[_defaultOutputs.Length];
            for (var i = 0; i < _defaultOutputs.Length; i++)
            {
                Outputs[i] = _defaultOutputs[i];
            }
            if (_init == null) return;
            _state = _init();
        }

        public void Update()
        {
            var result = _update(Inputs, _state);
            if (result.Length != Outputs.Length) throw Errors.PortCountMismatch(result.Length, Outputs.Length);
            Outputs = result;
        }

        // Basic nodes
        public static BsNode Bool(bool value)
        {
            var logicValue = BsMatrix.ToLogic(value);
            return new BsNode("bool", Array.Empty<float>(), new[] { logicValue },
                (_, _) => new[] { logicValue }, logicValue);
        }

        public static BsNode Int(int value)
        {
            var logicValue = BsMatrix.ToLogic(value);
            return new BsNode("int", Array.Empty<float>(), new[] { logicValue },
                (_, _) => new[] { logicValue }, logicValue);
        }

        public static BsNode Float(float value)
        {
            return new BsNode("float", Array.Empty<float>(), new[] { value },
                (_, _) => new[] { value }, value);
        }

        public static BsNode Randbool()
        {
            return new BsNode("randbool", Array.Empty<float>(), new float[1],
                (_, _) =>
                {
                    return new[] { BsMatrix.ToLogic(Random.Range(0, 2)) };
                });
        }

        public static BsNode Randint(int min, int max)
        {
            return new BsNode("randint", new[] { BsMatrix.ToLogic(min), BsMatrix.ToLogic(max) }, new float[1],
                (inputs, _) =>
                {
                    return new[] { BsMatrix.ToLogic(Random.Range(BsMatrix.ToInt(inputs[0]), BsMatrix.ToInt(inputs[1]) + 1)) };
                }, min, max);
        }

        public static BsNode Randfloat(float min, float max)
        {
            return new BsNode("randfloat", new[] { min, max }, new float[1],
                (inputs, _) =>
                {
                    return new[] { Random.Range(min, max) };
                }, min, max);
        }

        public static BsNode Mem()
        {
            return new BsNode("mem", new float[2], new float[1],
                () => new float[1], (inputs, state) =>
                {
                    if (!BsMatrix.ToBool(inputs[1])) return new[] { state[0] };
                    state[0] = inputs[0];
                    return new[] { state[0] };
                });
        }

        // Timing nodes
        public static BsNode Timer()
        {
            return new BsNode("timer", Array.Empty<float>(), new float[1],
                () => new[] { Time.timeSinceLevelLoad }, (_, state) =>
                {
                    return new[] { Time.timeSinceLevelLoad - state[0] };
                });
        }

        public static BsNode Clock(int interval)
        {
            return new BsNode("clock", Array.Empty<float>(), new float[1],
                () => new float[2], (_, state) =>
                {
                    var counter = BsMatrix.ToInt(state[0]);
                    if (counter >= interval)
                    {
                        state[1] = BsMatrix.ToLogic(!BsMatrix.ToBool(state[1]));
                        counter = 0;
                    }
                    counter++;
                    state[0] = BsMatrix.ToLogic(counter);
                    return new[] { state[1] };
                }, interval);
        }

        public static BsNode Onchanged()
        {
            return new BsNode("onchanged", new float[1], new float[1],
                () => new float[1], (inputs, state) =>
                {
                    if (Mathf.Approximately(inputs[0], state[0])) return new[] { 0f };
                    state[0] = inputs[0];
                    return new[] { 1f };
                });
        }

        // Control nodes
        public static BsNode Mux(int inputCount)
        {
            return new BsNode("mux", new float[inputCount + 1], new float[1], (inputs, _) =>
            {
                return new[] { inputs[BsMatrix.ToInt(inputs[0]) + 1] };
            }, inputCount);
        }

        public static BsNode Demux(int outputCount)
        {
            return new BsNode("demux", new float[2], new float[outputCount], (inputs, _) =>
            {
                var result = new float[outputCount];
                result[BsMatrix.ToInt(inputs[0])] = inputs[1];
                return result;
            }, outputCount);
        }

        // Logic nodes
        public static BsNode Buffer()
        {
            return new BsNode("buffer", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { inputs[0] };
            });
        }

        public static BsNode Not()
        {
            return new BsNode("not", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!BsMatrix.ToBool(inputs[0])) };
            });
        }

        public static BsNode And(int inputCount)
        {
            return new BsNode("and", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(inputs.Aggregate(true, (current, input) => current & BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Or(int inputCount)
        {
            return new BsNode("or", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(inputs.Aggregate(false, (current, input) => current | BsMatrix.ToBool(input))) };
            },inputCount);
        }

        public static BsNode Xor(int inputCount)
        {
            return new BsNode("xor", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(inputs.Aggregate(false, (current, input) => current ^ BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Nand(int inputCount)
        {
            return new BsNode("nand", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!inputs.Aggregate(true, (current, input) => current & BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Nor(int inputCount)
        {
            return new BsNode("nor", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!inputs.Aggregate(false, (current, input) => current | BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Xnor(int inputCount)
        {
            return new BsNode("xnor", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!inputs.Aggregate(false, (current, input) => current ^ BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        // Math nodes
        public static BsNode Add(int inputCount)
        {
            return new BsNode("add", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { inputs.Sum() };
            }, inputCount);
        }

        public static BsNode Subtract(int inputCount)
        {
            return new BsNode("subtract", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { inputs.Aggregate(0f, (current, input) => current - input) };
            }, inputCount);
        }

        public static BsNode Multiply(int inputCount)
        {
            return new BsNode("multiply", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { inputs.Aggregate(1f, (current, input) => current * input) };
            }, inputCount);
        }

        public static BsNode Divide(int inputCount)
        {
            return new BsNode("divide", new float[inputCount], new float[1],
                () => new[] { BsMatrix.ToLogic(inputCount) }, (inputs, _) =>
            {
                return new[] { inputs.Aggregate(1f, (current, input) => current / input) };
            }, inputCount);
        }

        public static BsNode Pow()
        {
            return new BsNode("pow", new float[2], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Pow(inputs[0], inputs[1]) };
            });
        }

        public static BsNode Root()
        {
            return new BsNode("root", new float[2], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Pow(inputs[0], 1f / inputs[1]) };
            });
        }

        public static BsNode Sin()
        {
            return new BsNode("sin", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Sin(inputs[0]) };
            });
        }

        public static BsNode Cos()
        {
            return new BsNode("cos", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Cos(inputs[0]) };
            });
        }

        public static BsNode Tan()
        {
            return new BsNode("tan", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Tan(inputs[0]) };
            });
        }

        public static BsNode Asin()
        {
            return new BsNode("asin", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Asin(inputs[0]) };
            });
        }

        public static BsNode Acos()
        {
            return new BsNode("acos", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Acos(inputs[0]) };
            });
        }

        public static BsNode Atan()
        {
            return new BsNode("atan", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Atan(inputs[0]) };
            });
        }

        public LcsLine ToLcs()
        {
            var properties = new string[Config.Length];
            for (var i = 0; i < Config.Length; i++)
            {
                properties[i] = LcsParser.Stringify(Config[i]);
            }
            return new LcsLine
            (
                '%',
                new[] { LcsParser.Stringify(Tag) },
                properties
            );
        }

        public static BsNode FromLcs(LcsLine line)
        {
            var tag = LcsParser.ParseString(line.Header[0]);
            var config = new float[line.Properties.Length];
            for (var i = 0; i < line.Properties.Length; i++)
            {
                config[i] = LcsParser.ParseFloat(line.Properties[i]);
            }
            return tag switch
            {
                "bool" => Bool(BsMatrix.ToBool(config[0])),
                "int" => Int(BsMatrix.ToInt(config[0])),
                "float" => Float(config[0]),
                "randbool" => Randbool(),
                "randint" => Randint(BsMatrix.ToInt(config[0]), BsMatrix.ToInt(config[1])),
                "randfloat" => Randfloat(config[0], config[1]),
                "mem" => Mem(),
                "timer" => Timer(),
                "clock" => Clock(BsMatrix.ToInt(config[0])),
                "onchanged" => Onchanged(),
                "mux" => Mux(BsMatrix.ToInt(config[0])),
                "demux" => Demux(BsMatrix.ToInt(config[0])),
                "buffer" => Buffer(),
                "not" => Not(),
                "and" => And(BsMatrix.ToInt(config[0])),
                "or" => Or(BsMatrix.ToInt(config[0])),
                "xor" => Xor(BsMatrix.ToInt(config[0])),
                "nand" => Nand(BsMatrix.ToInt(config[0])),
                "nor" => Nor(BsMatrix.ToInt(config[0])),
                "xnor" => Xnor(BsMatrix.ToInt(config[0])),
                "add" => Add(BsMatrix.ToInt(config[0])),
                "subtract" => Subtract(BsMatrix.ToInt(config[0])),
                "multiply" => Multiply(BsMatrix.ToInt(config[0])),
                "divide" => Divide(BsMatrix.ToInt(config[0])),
                "pow" => Pow(),
                "root" => Root(),
                "sin" => Sin(),
                "cos" => Cos(),
                "tan" => Tan(),
                "asin" => Asin(),
                "acos" => Acos(),
                _ => throw Errors.InvalidTag("node", tag)
            };
        }
    }
}
