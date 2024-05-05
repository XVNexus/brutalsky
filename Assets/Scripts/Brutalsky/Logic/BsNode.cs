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
        public static BsNode ConstantBool(bool value)
        {
            var logicValue = BsMatrix.ToLogic(value);
            return new BsNode("bcbl", Array.Empty<float>(), new[] { logicValue },
                (_, _) => new[] { logicValue }, logicValue);
        }

        public static BsNode ConstantInt(int value)
        {
            var logicValue = BsMatrix.ToLogic(value);
            return new BsNode("bcin", Array.Empty<float>(), new[] { logicValue },
                (_, _) => new[] { logicValue }, logicValue);
        }

        public static BsNode ConstantFloat(float value)
        {
            return new BsNode("bcfl", Array.Empty<float>(), new[] { value },
                (_, _) => new[] { value }, value);
        }

        public static BsNode RandomBool()
        {
            return new BsNode("brbl", new float[1], new float[1],
                () => new[] { BsMatrix.ToLogic(Random.Range(0, 2)) }, (inputs, state) =>
                {
                    if (BsMatrix.ToBool(inputs[0]))
                    {
                        state[0] = BsMatrix.ToLogic(Random.Range(0, 2));
                    }
                    return new[] { state[0] };
                });
        }

        public static BsNode RandomInt(int min, int max)
        {
            return new BsNode("brin", new[] { 0f, BsMatrix.ToLogic(min), BsMatrix.ToLogic(max) }, new float[1],
                () => new[] { BsMatrix.ToLogic(Random.Range(min, max + 1)) }, (inputs, state) =>
                {
                    if (BsMatrix.ToBool(inputs[0]))
                    {
                        state[0] = BsMatrix.ToLogic(Random.Range(BsMatrix.ToInt(inputs[1]), BsMatrix.ToInt(inputs[2]) + 1));
                    }
                    return new[] { state[0] };
                }, min, max);
        }

        public static BsNode RandomFloat(float min, float max)
        {
            return new BsNode("brfl", new[] { 0f, min, max }, new float[1],
                () => new[] { Random.Range(min, max) }, (inputs, state) =>
                {
                    if (BsMatrix.ToBool(inputs[0]))
                    {
                        state[0] = Random.Range(inputs[1], inputs[2]);
                    }
                    return new[] { state[0] };
                }, min, max);
        }

        public static BsNode MemoryCell()
        {
            return new BsNode("bmem", new float[2], new float[1],
                () => new float[1], (inputs, state) =>
                {
                    if (!BsMatrix.ToBool(inputs[1])) return new[] { state[0] };
                    state[0] = inputs[0];
                    return new[] { state[0] };
                });
        }

        // Flow nodes
        public static BsNode GameTime()
        {
            return new BsNode("ftmr", Array.Empty<float>(), new float[1],
                () => new[] { Time.timeSinceLevelLoad }, (_, state) =>
                {
                    return new[] { Time.timeSinceLevelLoad - state[0] };
                });
        }

        public static BsNode Delayer(int time)
        {
            return new BsNode("fdly", new float[1], new float[1],
                () => new float[time], (inputs, state) =>
                {
                    for (var i = time - 1; i >= 1; i--)
                    {
                        state[i] = state[i - 1];
                    }
                    state[0] = inputs[0];
                    return new[] { state[^1] };
                }, time);
        }

        public static BsNode Clock(int interval)
        {
            return new BsNode("fclk", Array.Empty<float>(), new float[1],
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

        public static BsNode ChangeListener()
        {
            return new BsNode("fchl", new float[1], new float[1],
                () => new float[1], (inputs, state) =>
                {
                    if (Mathf.Approximately(inputs[0], state[0])) return new[] { 0f };
                    state[0] = inputs[0];
                    return new[] { 1f };
                });
        }

        public static BsNode Multiplexer(int inputCount)
        {
            return new BsNode("fmux", new float[inputCount + 1], new float[1], (inputs, _) =>
            {
                return new[] { inputs[BsMatrix.ToInt(inputs[0]) + 1] };
            }, inputCount);
        }

        public static BsNode Demultiplexer(int outputCount)
        {
            return new BsNode("fdmx", new float[2], new float[outputCount], (inputs, _) =>
            {
                var result = new float[outputCount];
                result[BsMatrix.ToInt(inputs[0])] = inputs[1];
                return result;
            }, outputCount);
        }

        // Logic nodes
        public static BsNode Buffer()
        {
            return new BsNode("lbuf", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { inputs[0] };
            });
        }

        public static BsNode Not()
        {
            return new BsNode("lnot", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!BsMatrix.ToBool(inputs[0])) };
            });
        }

        public static BsNode And(int inputCount)
        {
            return new BsNode("land", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(inputs.Aggregate(true, (current, input) => current & BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Or(int inputCount)
        {
            return new BsNode("lgor", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(inputs.Aggregate(false, (current, input) => current | BsMatrix.ToBool(input))) };
            },inputCount);
        }

        public static BsNode Xor(int inputCount)
        {
            return new BsNode("lxor", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(inputs.Aggregate(false, (current, input) => current ^ BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Nand(int inputCount)
        {
            return new BsNode("lnan", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!inputs.Aggregate(true, (current, input) => current & BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Nor(int inputCount)
        {
            return new BsNode("lnor", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!inputs.Aggregate(false, (current, input) => current | BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        public static BsNode Xnor(int inputCount)
        {
            return new BsNode("lxnr", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { BsMatrix.ToLogic(!inputs.Aggregate(false, (current, input) => current ^ BsMatrix.ToBool(input))) };
            }, inputCount);
        }

        // Math nodes
        public static BsNode Add(int inputCount)
        {
            return new BsNode("madd", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { inputs.Sum() };
            }, inputCount);
        }

        public static BsNode Subtract(int inputCount)
        {
            return new BsNode("msub", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { inputs.Aggregate(0f, (current, input) => current - input) };
            }, inputCount);
        }

        public static BsNode Multiply(int inputCount)
        {
            return new BsNode("mmul", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { inputs.Aggregate(1f, (current, input) => current * input) };
            }, inputCount);
        }

        public static BsNode Divide(int inputCount)
        {
            return new BsNode("mdiv", new float[inputCount], new float[1], (inputs, _) =>
            {
                return new[] { inputs.Aggregate(1f, (current, input) => current / input) };
            }, inputCount);
        }

        public static BsNode Pow()
        {
            return new BsNode("mpow", new float[2], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Pow(inputs[0], inputs[1]) };
            });
        }

        public static BsNode Root()
        {
            return new BsNode("mrot", new float[2], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Pow(inputs[0], 1f / inputs[1]) };
            });
        }

        public static BsNode Sin()
        {
            return new BsNode("msin", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Sin(inputs[0]) };
            });
        }

        public static BsNode Cos()
        {
            return new BsNode("mcos", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Cos(inputs[0]) };
            });
        }

        public static BsNode Tan()
        {
            return new BsNode("mtan", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Tan(inputs[0]) };
            });
        }

        public static BsNode Asin()
        {
            return new BsNode("masn", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Asin(inputs[0]) };
            });
        }

        public static BsNode Acos()
        {
            return new BsNode("macs", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Acos(inputs[0]) };
            });
        }

        public static BsNode Atan()
        {
            return new BsNode("matn", new float[1], new float[1], (inputs, _) =>
            {
                return new[] { Mathf.Atan(inputs[0]) };
            });
        }

        public LcsLine ToLcs()
        {
            var properties = new string[Config.Length + 1];
            properties[0] = Stringifier.Str(LcsType.String, Tag);
            for (var i = 0; i < Config.Length; i++)
            {
                properties[i + 1] = Stringifier.Str(LcsType.Float, Config[i]);
            }
            return new LcsLine('%', properties);
        }

        public static BsNode FromLcs(LcsLine line)
        {
            var tag = Stringifier.Par<string>(LcsType.String, line.Properties[0]);
            var config = new float[line.Properties.Length - 1];
            for (var i = 1; i < line.Properties.Length; i++)
            {
                config[i - 1] = Stringifier.Par<float>(LcsType.Float, line.Properties[i]);
            }
            return tag switch
            {
                // Basic nodes
                "bcbl" => ConstantBool(BsMatrix.ToBool(config[0])),
                "bcin" => ConstantInt(BsMatrix.ToInt(config[0])),
                "bcfl" => ConstantFloat(config[0]),
                "brbl" => RandomBool(),
                "brin" => RandomInt(BsMatrix.ToInt(config[0]), BsMatrix.ToInt(config[1])),
                "brfl" => RandomFloat(config[0], config[1]),
                "bmem" => MemoryCell(),
                // Flow nodes
                "ftmr" => GameTime(),
                "fdly" => Delayer(BsMatrix.ToInt(config[0])),
                "fclk" => Clock(BsMatrix.ToInt(config[0])),
                "fchl" => ChangeListener(),
                "fmux" => Multiplexer(BsMatrix.ToInt(config[0])),
                "fdmx" => Demultiplexer(BsMatrix.ToInt(config[0])),
                // Logic nodes
                "lbuf" => Buffer(),
                "lnot" => Not(),
                "land" => And(BsMatrix.ToInt(config[0])),
                "lgor" => Or(BsMatrix.ToInt(config[0])),
                "lxor" => Xor(BsMatrix.ToInt(config[0])),
                "lnan" => Nand(BsMatrix.ToInt(config[0])),
                "lnor" => Nor(BsMatrix.ToInt(config[0])),
                "lxnr" => Xnor(BsMatrix.ToInt(config[0])),
                // Math nodes
                "madd" => Add(BsMatrix.ToInt(config[0])),
                "msub" => Subtract(BsMatrix.ToInt(config[0])),
                "mmul" => Multiply(BsMatrix.ToInt(config[0])),
                "mdiv" => Divide(BsMatrix.ToInt(config[0])),
                "mpow" => Pow(),
                "mrot" => Root(),
                "msin" => Sin(),
                "mcos" => Cos(),
                "mtan" => Tan(),
                "masn" => Asin(),
                "macs" => Acos(),
                "matn" => Atan(),
                _ => throw Errors.InvalidItem("node tag", tag)
            };
        }
    }
}
