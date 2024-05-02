using System;
using Core;

namespace Brutalsky.Logic
{
    public class BsNode
    {
        public string Id { get => _id; set => _id = MapSystem.CleanId(value); }
        private string _id;
        public float[] Inputs { get; set; }
        public float[] Outputs { get; private set; }
        private Func<float[], float[]> _update;

        public BsNode(string id, float[] inputs, float[] outputs, Func<float[], float[]> update)
        {
            Id = id;
            Inputs = inputs;
            Outputs = outputs;
            _update = update;
        }

        public void Update()
        {
            Outputs = _update(Inputs);
        }
    }
}
