using System.Collections.Generic;
using Utils;

namespace Data.Logic
{
    public class BsMatrix
    {
        private readonly IdList<BsNode> _nodes = new();
        private readonly Dictionary<BsPort, BsPort> _links = new();
        private readonly Dictionary<BsPort, float> _buffer = new();

        public BsMatrix(IEnumerable<BsNode> nodes, IEnumerable<BsLink> links)
        {
            foreach (var node in nodes)
            {
                node.Reset();
                _nodes.Add(node);
            }
            foreach (var link in links)
            {
                _links[link.ToPort] = link.FromPort;
                _buffer[link.ToPort] = float.NaN;
            }
        }

        public void Update()
        {
            foreach (var node in _nodes.Values)
            {
                node.Step();
            }
            foreach (var toPort in _links.Keys)
            {
                var fromPort = _links[toPort];
                _buffer[toPort] = _nodes[fromPort.NodeId].Outputs[fromPort.PortId];
            }
            foreach (var toPort in _buffer.Keys)
            {
                _nodes[toPort.NodeId].Inputs[toPort.PortId] = _buffer[toPort];
            }
        }
    }
}
