using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky.Logic
{
    public class BsMatrix
    {
        public Dictionary<string, BsNode> Nodes { get; } = new();
        public Dictionary<string, BsPort> Ports { get; } = new();
        public Dictionary<string, string> Links { get; } = new();
        private Dictionary<string, float> _buffer = new();

        public void Update()
        {
            foreach (var key in Links.Keys)
            {
                _buffer[key] = Ports[Links[key]].ValueIfChanged;
                if (!float.IsNaN(_buffer[key]))
                {
                    var port = Ports[Links[key]];
                    Debug.Log($"{port.Id} = {port.Value}");
                }
            }
            foreach (var key in _buffer.Keys.Where(key => !float.IsNaN(_buffer[key])))
            {
                Debug.Log($"{key} --> {_buffer[key]}");
                Ports[key].Value = _buffer[key];
                Ports[Links[key]].Changed = false;
            }
            foreach (var node in Nodes.Values)
            {
                node.Update();
            }
        }

        public void Init()
        {
            foreach (var node in Nodes.Values)
            {
                node.RegisterLogic(this);
            }
        }

        public void Reset()
        {
            ClearPorts();
            ClearLinks();
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

        public string GetLink(string toId)
        {
            return ContainsLink(toId) ? Links[toId] : "";
        }

        public void AddLink(string fromId, string toId)
        {
            Links[toId] = fromId;
        }

        public bool RemoveLink(string toId)
        {
            return Links.Remove(toId);
        }

        public bool RemoveLink(string fromId, string toId)
        {
            return ContainsLink(fromId, toId) && Links.Remove(toId);
        }

        public bool ContainsLink(string toId)
        {
            return Links.ContainsKey(toId);
        }

        public bool ContainsLink(string fromId, string toId)
        {
            return Links.ContainsKey(toId) && Links[toId] == fromId;
        }

        public void ClearLinks()
        {
            Links.Clear();
        }

        [CanBeNull]
        public BsPort GetPort(string id)
        {
            return ContainsPort(id) ? Ports[id] : null;
        }

        public void AddPort(BsPort port)
        {
            Ports[port.Id] = port;
            _buffer[port.Id] = float.NaN;
        }

        public bool RemovePort(string id)
        {
            if (!ContainsPort(id)) return false;
            Ports.Remove(id);
            _buffer.Remove(id);
            return true;
        }

        public bool ContainsPort(string id)
        {
            return Ports.ContainsKey(id);
        }

        public void ClearPorts()
        {
            Ports.Clear();
            _buffer.Clear();
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
