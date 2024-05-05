using System.Collections.Generic;
using UnityEngine;
using Utils.Constants;

namespace Brutalsky.Logic
{
    public class BsMatrix
    {
        private readonly List<BsNode> _nodes = new();
        private readonly Dictionary<(int, int), (int, int)> _links = new();
        private readonly Dictionary<(int, int), float> _buffer = new();

        public BsMatrix(IEnumerable<BsNode> nodes, IEnumerable<BsLink> links)
        {
            _nodes.AddRange(nodes);
            foreach (var node in _nodes)
            {
                node.Init();
            }
            foreach (var link in links)
            {
                _links[link.ToPort] = link.FromPort;
            }
            foreach (var toPort in _links.Keys)
            {
                _buffer[toPort] = float.NaN;
            }
        }

        public void Update()
        {
            foreach (var node in _nodes)
            {
                node.Update();
            }
            foreach (var toPort in _links.Keys)
            {
                _buffer[toPort] = GetPort(_links[toPort]);
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
            return _nodes[id].Outputs[index];
        }

        public void SetPort((int, int) port, float value)
        {
            SetPort(port.Item1, port.Item2, value);
        }

        public void SetPort(int id, int index, float value)
        {
            ValidateInputPort(id, index);
            _nodes[id].Inputs[index] = value;
        }

        public bool ContainsInputPort(int id, int index)
        {
            return ContainsNode(id) && index >= 0 && index < _nodes[id].Inputs.Length;
        }

        public bool ContainsOutputPort(int id, int index)
        {
            return ContainsNode(id) && index >= 0 && index < _nodes[id].Outputs.Length;
        }

        public void ValidateInputPort(int id, int index)
        {
            if (!ContainsNode(id)) throw Errors.NoItemFound("node", id);
            if (!ContainsInputPort(id, index)) throw Errors.NoItemFound("input port", $"{id}:{index}");
        }

        public void ValidateOutputPort(int id, int index)
        {
            if (!ContainsNode(id)) throw Errors.NoItemFound("node", id);
            if (!ContainsOutputPort(id, index)) throw Errors.NoItemFound("output port", $"{id}:{index}");
        }

        public bool ContainsNode(int id)
        {
            return id >= 0 && id < _nodes.Count;
        }

        public bool ContainsLink((int, int) toPort)
        {
            return _links.ContainsKey(toPort);
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
