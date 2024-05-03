using System;
using System.Linq;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;

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
            return new BsNode("bool", new float[0], new[] { logicValue },
                (_, _) => new[] { logicValue }, logicValue);
        }

        public static BsNode Int(int value)
        {
            var logicValue = BsMatrix.ToLogic(value);
            return new BsNode("int", new float[0], new[] { logicValue },
                (_, _) => new[] { logicValue }, logicValue);
        }

        public static BsNode Float(float value)
        {
            return new BsNode("float", new float[0], new[] { value },
                (_, _) => new[] { value }, value);
        }

        // System nodes
        public static BsNode Timer()
        {
            return new BsNode("timer", new float[0], new float[1],
                () => new[] { Time.timeSinceLevelLoad }, (_, state) =>
            {
                return new[] { Time.timeSinceLevelLoad - state[0] };
            });
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
                "timer" => Timer(),
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
