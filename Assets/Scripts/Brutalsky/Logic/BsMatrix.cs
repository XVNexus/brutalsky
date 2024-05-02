using System.Collections.Generic;
using UnityEngine;
using Utils.Constants;

namespace Brutalsky.Logic
{
    public class BsMatrix
    {
        public List<BsNode> Nodes { get; }
        public Dictionary<(int, int), (int, int)> Links { get; }
        private Dictionary<(int, int), float> _buffer = new();

        public BsMatrix(List<BsNode> nodes, Dictionary<(int, int), (int, int)> links)
        {
            Nodes = nodes;
            Links = links;
            foreach (var toPort in Links.Keys)
            {
                _buffer[toPort] = float.NaN;
            }
        }

        public void Update()
        {
            foreach (var node in Nodes)
            {
                node.Update();
            }
            foreach (var toPort in Links.Keys)
            {
                _buffer[toPort] = GetPort(Links[toPort]);
            }
            foreach (var toPort in _buffer.Keys)
            {
                SetPort(toPort, _buffer[toPort]);
            }
        }

        public float GetPort((int, int) port)
        {
            return GetPort(port.Item1, port.Item2);
        }

        public float GetPort(int id, int index)
        {
            ValidateOutputPort(id, index);
            return Nodes[id].Outputs[index];
        }

        public void SetPort((int, int) port, float value)
        {
            SetPort(port.Item1, port.Item2, value);
        }

        public void SetPort(int id, int index, float value)
        {
            ValidateInputPort(id, index);
            Nodes[id].Inputs[index] = value;
        }

        public bool ContainsInputPort(int id, int index)
        {
            return ContainsNode(id) && index >= 0 && index < Nodes[id].Inputs.Length;
        }

        public bool ContainsOutputPort(int id, int index)
        {
            return ContainsNode(id) && index >= 0 && index < Nodes[id].Outputs.Length;
        }

        public void ValidateInputPort(int id, int index)
        {
            if (!ContainsNode(id)) throw Errors.NoNodeFound(id);
            if (!ContainsInputPort(id, index)) throw Errors.NoPortFound("input", id, index);
        }

        public void ValidateOutputPort(int id, int index)
        {
            if (!ContainsNode(id)) throw Errors.NoNodeFound(id);
            if (!ContainsOutputPort(id, index)) throw Errors.NoPortFound("output", id, index);
        }

        public bool ContainsNode(int id)
        {
            return id >= 0 && id < Nodes.Count;
        }

        public bool ContainsLink((int, int) toPort)
        {
            return Links.ContainsKey(toPort);
        }

        public static float Bool2Logic(bool value)
        {
            return value ? 1f : 0f;
        }

        public static bool Logic2Bool(float logic)
        {
            return logic >= .5f;
        }
    }
}
