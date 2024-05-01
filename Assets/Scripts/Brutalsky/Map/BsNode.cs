using System;

namespace Brutalsky.Map
{
    public class BsNode
    {
        public string Id { get; private set; }

        private Func<float[], float[]> _compute;

        public BsNode(string id, Func<float[], float[]> compute)
        {
            Id = id;
            _compute = compute;
        }

        public float[] Compute(float[] inputs)
        {
            return _compute(inputs);
        }
    }
}
