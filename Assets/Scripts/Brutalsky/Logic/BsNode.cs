using System;
using Core;

namespace Brutalsky.Logic
{
    public class BsNode
    {
        public string Id { get => _id; set => _id = MapSystem.CleanId(value); }
        private string _id;
        public BsPort[] Inputs { get; private set; }
        public BsPort[] Outputs { get; private set; }
        private Func<BsPort[], float[]> _update;

        public BsNode(string id, int inputCount, int outputCount, Func<BsPort[], float[]> update)
        {
            Id = id;
            Inputs = new BsPort[inputCount];
            for (var i = 0; i < inputCount; i++)
            {
                Inputs[i] = new BsPort(id, $"i{i}", false);
            }
            Outputs = new BsPort[outputCount];
            for (var i = 0; i < outputCount; i++)
            {
                Outputs[i] = new BsPort(id, $"o{i}", false);
            }
            _update = update;
        }

        public void RegisterLogic(BsMatrix matrix)
        {
            foreach (var port in Inputs)
            {
                matrix.AddPort(port);
            }
            foreach (var port in Outputs)
            {
                matrix.AddPort(port);
            }
        }

        public void Update()
        {
            var values = _update(Inputs);
            for (var i = 0; i < values.Length; i++)
            {
                Outputs[i].Value = values[i];
            }
        }
    }
}
