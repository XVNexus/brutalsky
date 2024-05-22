using System.Collections.Generic;
using Brutalsky.Scripts.Map;
using UnityEngine;
using Utils.Constants;

namespace Brutalsky.Logic
{
    public class BsMatrix
    {
        private readonly List<BsNode> _nodes = new();
        private readonly Dictionary<BsPort, BsPort> _links = new();
        private readonly Dictionary<BsPort, float> _buffer = new();

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

        public float GetPort(BsPort port)
        {
            ValidateOutputPort(port);
            return _nodes[port.NodeId].Outputs[port.PortId];
        }

        public void SetPort(BsPort port, float value)
        {
            ValidateInputPort(port);
            _nodes[port.NodeId].Inputs[port.PortId] = value;
        }

        public bool ContainsInputPort(BsPort port)
        {
            return ContainsNode(port.NodeId) && port.PortId >= 0 && port.PortId < _nodes[port.NodeId].Inputs.Length;
        }

        public bool ContainsOutputPort(BsPort port)
        {
            return ContainsNode(port.NodeId) && port.PortId >= 0 && port.PortId < _nodes[port.NodeId].Outputs.Length;
        }

        public void ValidateInputPort(BsPort port)
        {
            if (!ContainsNode(port.NodeId)) throw Errors.NoItemFound("node", port.NodeId);
            if (!ContainsInputPort(port)) throw Errors.NoItemFound("input port", $"{port.NodeId}:{port.PortId}");
        }

        public void ValidateOutputPort(BsPort port)
        {
            if (!ContainsNode(port.NodeId)) throw Errors.NoItemFound("node", port.NodeId);
            if (!ContainsOutputPort(port)) throw Errors.NoItemFound("output port", $"{port.NodeId}:{port.PortId}");
        }

        public bool ContainsNode(ushort id)
        {
            return id >= 0 && id < _nodes.Count;
        }

        public bool ContainsLink(BsPort toPort)
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
