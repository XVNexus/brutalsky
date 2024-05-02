using System;
using System.Linq;
using UnityEngine;
using Utils.Constants;

namespace Brutalsky.Logic
{
    public class BsNode
    {
        public string Tag { get; set; }
        public float[] Inputs { get; set; }
        public float[] Outputs { get; private set; }
        private Func<float[], float[]> _update;

        public BsNode(string tag, float[] inputs, float[] outputs, Func<float[], float[]> update)
        {
            Tag = tag;
            Inputs = inputs;
            Outputs = outputs;
            _update = update;
        }

        public BsNode(float[] inputs, float[] outputs, Func<float[], float[]> update)
        {
            Inputs = inputs;
            Outputs = outputs;
            _update = update;
        }

        public void Update()
        {
            var result = _update(Inputs);
            if (result.Length != Outputs.Length) throw Errors.PortCountMismatch(result.Length, Outputs.Length);
            Outputs = result;
        }

        // Basic nodes
        public static BsNode Constant(bool value)
        {
            return Constant(BsMatrix.Bool2Logic(value));
        }

        public static BsNode Constant(float value)
        {
            return new BsNode("constant", new float[0], new[] { value }, _ => new[] { value });
        }

        public static BsNode LevelTime()
        {
            return new BsNode("level-time", new float[0], new float[1], _ => new[] { Time.timeSinceLevelLoad });
        }

        // Logic nodes
        public static BsNode Buffer()
        {
            return new BsNode("buffer", new float[1], new float[1], inputs =>
            {
                return new[] { inputs[0] };
            });
        }

        public static BsNode Not()
        {
            return new BsNode("not", new float[1], new float[1], inputs =>
            {
                return new[] { BsMatrix.Bool2Logic(!BsMatrix.Logic2Bool(inputs[0])) };
            });
        }

        public static BsNode And(int inputCount)
        {
            return new BsNode("and", new float[inputCount], new float[1], inputs =>
            {
                return new[] { BsMatrix.Bool2Logic(inputs.Aggregate(true, (current, input) => current & BsMatrix.Logic2Bool(input))) };
            });
        }

        public static BsNode Or(int inputCount)
        {
            return new BsNode("or", new float[inputCount], new float[1], inputs =>
            {
                return new[] { BsMatrix.Bool2Logic(inputs.Aggregate(false, (current, input) => current | BsMatrix.Logic2Bool(input))) };
            });
        }

        public static BsNode Xor(int inputCount)
        {
            return new BsNode("xor", new float[inputCount], new float[1], inputs =>
            {
                return new[] { BsMatrix.Bool2Logic(inputs.Aggregate(false, (current, input) => current ^ BsMatrix.Logic2Bool(input))) };
            });
        }

        public static BsNode Nand(int inputCount)
        {
            return new BsNode("nand", new float[inputCount], new float[1], inputs =>
            {
                return new[] { BsMatrix.Bool2Logic(!inputs.Aggregate(true, (current, input) => current & BsMatrix.Logic2Bool(input))) };
            });
        }

        public static BsNode Nor(int inputCount)
        {
            return new BsNode("nor", new float[inputCount], new float[1], inputs =>
            {
                return new[] { BsMatrix.Bool2Logic(!inputs.Aggregate(false, (current, input) => current | BsMatrix.Logic2Bool(input))) };
            });
        }

        public static BsNode Xnor(int inputCount)
        {
            return new BsNode("xnor", new float[inputCount], new float[1], inputs =>
            {
                return new[] { BsMatrix.Bool2Logic(!inputs.Aggregate(false, (current, input) => current ^ BsMatrix.Logic2Bool(input))) };
            });
        }

        // Math nodes
        public static BsNode Add(int inputCount)
        {
            return new BsNode("add", new float[inputCount], new float[1], inputs =>
            {
                return new[] { inputs.Sum() };
            });
        }

        public static BsNode Subtract(int inputCount)
        {
            return new BsNode("subtract", new float[inputCount], new float[1], inputs =>
            {
                return new[] { inputs.Aggregate(0f, (current, input) => current - input) };
            });
        }

        public static BsNode Multiply(int inputCount)
        {
            return new BsNode("multiply", new float[inputCount], new float[1], inputs =>
            {
                return new[] { inputs.Aggregate(1f, (current, input) => current * input) };
            });
        }

        public static BsNode Divide(int inputCount)
        {
            return new BsNode("divide", new float[inputCount], new float[1], inputs =>
            {
                return new[] { inputs.Aggregate(1f, (current, input) => current / input) };
            });
        }

        public static BsNode Pow()
        {
            return new BsNode("pow", new float[2], new float[1], inputs =>
            {
                return new[] { Mathf.Pow(inputs[0], inputs[1]) };
            });
        }

        public static BsNode Root()
        {
            return new BsNode("root", new float[2], new float[1], inputs =>
            {
                return new[] { Mathf.Pow(inputs[0], 1f / inputs[1]) };
            });
        }

        public static BsNode Sin()
        {
            return new BsNode("sin", new float[1], new float[1], inputs =>
            {
                return new[] { Mathf.Sin(inputs[0]) };
            });
        }

        public static BsNode Cos()
        {
            return new BsNode("cos", new float[1], new float[1], inputs =>
            {
                return new[] { Mathf.Cos(inputs[0]) };
            });
        }

        public static BsNode Tan()
        {
            return new BsNode("tan", new float[1], new float[1], inputs =>
            {
                return new[] { Mathf.Tan(inputs[0]) };
            });
        }

        public static BsNode Asin()
        {
            return new BsNode("asin", new float[1], new float[1], inputs =>
            {
                return new[] { Mathf.Asin(inputs[0]) };
            });
        }

        public static BsNode Acos()
        {
            return new BsNode("acos", new float[1], new float[1], inputs =>
            {
                return new[] { Mathf.Acos(inputs[0]) };
            });
        }

        public static BsNode Atan()
        {
            return new BsNode("atan", new float[1], new float[1], inputs =>
            {
                return new[] { Mathf.Atan(inputs[0]) };
            });
        }
    }
}
