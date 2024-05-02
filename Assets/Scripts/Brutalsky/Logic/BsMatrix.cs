using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky.Logic
{
    public class BsMatrix
    {
        public Dictionary<string, BsNode> Nodes { get; } = new();
        public Dictionary<(string, int), (string, int)> Links { get; } = new();
        private Dictionary<(string, int), float> _buffer = new();

        public void Update()
        {
            foreach (var node in Nodes.Values)
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

        public float GetPort((string, int) port)
        {
            return GetPort(port.Item1, port.Item2);
        }

        public float GetPort(string id, int index)
        {
            return ContainsOutputPort(id, index) ? Nodes[id].Outputs[index] : float.NaN;
        }

        public bool SetPort((string, int) port, float value)
        {
            return SetPort(port.Item1, port.Item2, value);
        }

        public bool SetPort(string id, int index, float value)
        {
            if (!ContainsInputPort(id, index)) return false;
            Nodes[id].Inputs[index] = value;
            return true;
        }

        public bool ContainsInputPort(string id, int index)
        {
            return ContainsNode(id) && index >= 0 && index < Nodes[id].Inputs.Length;
        }

        public bool ContainsOutputPort(string id, int index)
        {
            return ContainsNode(id) && index >= 0 && index < Nodes[id].Outputs.Length;
        }

        [CanBeNull]
        public BsNode GetNode(string id)
        {
            return ContainsNode(id) ? Nodes[id] : null;
        }

        public bool AddNode(BsNode node)
        {
            if (ContainsNode(node)) return false;
            Nodes[node.Id] = node;
            return true;
        }

        public bool RemoveNode(BsNode node)
        {
            return RemoveNode(node.Id);
        }

        public bool RemoveNode(string id)
        {
            return Nodes.Remove(id);
        }

        public bool ContainsNode(BsNode node)
        {
            return ContainsNode(node.Id);
        }

        public bool ContainsNode(string id)
        {
            return Nodes.ContainsKey(id);
        }

        public (string, int) GetLink((string, int) toPort)
        {
            return ContainsLink(toPort) ? Links[toPort] : ("", -1);
        }

        public bool AddLink((string, int) fromPort, (string, int) toPort)
        {
            if (ContainsLink(toPort)) return false;
            Links[toPort] = fromPort;
            _buffer[toPort] = float.NaN;
            return true;
        }

        public bool RemoveLink((string, int) toPort)
        {
            if (!ContainsLink(toPort)) return false;
            Links.Remove(toPort);
            _buffer.Remove(toPort);
            return true;
        }

        public bool RemoveLink((string, int) fromPort, (string, int) toPort)
        {
            return ContainsLink(fromPort, toPort) && Links.Remove(toPort);
        }

        public bool ContainsLink((string, int) toPort)
        {
            return Links.ContainsKey(toPort);
        }

        public bool ContainsLink((string, int) fromPort, (string, int) toPort)
        {
            return ContainsLink(toPort) && Equals(Links[toPort], fromPort);
        }

        public void ClearLinks()
        {
            Links.Clear();
        }

        public static float Bool2Logic(bool value)
        {
            return value ? 1f : 0f;
        }

        public static bool Logic2Bool(float logic)
        {
            return logic >= .5f;
        }

        public static float Int2Logic(int value)
        {
            return value;
        }

        public static int Logic2Int(float logic)
        {
            return Mathf.RoundToInt(logic);
        }
    }
}
