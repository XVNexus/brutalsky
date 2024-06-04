using System.Collections.Generic;
using System.Linq;
using Data.Base;
using Extensions;
using Lcs;
using Systems;
using UnityEngine;
using Utils;
using Color = UnityEngine.Color;

namespace Data
{
    public class BsMap : ILcsDocument
    {
        public const byte DirectionNone = 0;
        public const byte DirectionDown = 1;
        public const byte DirectionUp = 2;
        public const byte DirectionLeft = 3;
        public const byte DirectionRight = 4;

        public uint Id => MapSystem.GenerateId(Title, Author);
        public string Title { get; set; }
        public string Author { get; set; }
        public Rect PlayArea { get; set; } = new(-10f, -10f, 20f, 20f);
        public Color BackgroundColor { get; set; } = Color.white.MultiplyTint(.25f);
        public Color LightingColor = Color.white.SetAlpha(.8f);
        public byte GravityDirection { get; set; } = DirectionNone;
        public float GravityStrength { get; set; }
        public float AirResistance { get; set; }
        public float PlayerHealth { get; set; } = 100f;
        public bool AllowDummies { get; set; } = true;

        public List<BsSpawn> Spawns { get; } = new();
        public IdList<BsObject> Objects { get; } = new();
        public List<BsNode> Nodes { get; } = new();
        public List<BsLink> Links { get; } = new();

        public List<BsNode> ActiveNodes { get; } = new();
        public Dictionary<(int, string), BsPort> ActivePorts { get; } = new();
        public Dictionary<(int, string), object> LinkBuffer { get; } = new();
        public bool MatrixActive { get; private set; }

        public BsMap(string title = "Untitled Map", string author = "Anonymous Marble")
        {
            Title = title;
            Author = author;
        }

        public BsMap() { }

        public Vector2 SelectSpawn()
        {
            var leastUsages = Spawns.Select(spawn => spawn.Usages).Prepend(int.MaxValue).Min();
            var possibleSpawns = Spawns.Where(spawn => spawn.Usages == leastUsages).ToList();
            possibleSpawns.Sort((a, b) => a.Priority - b.Priority);
            var lowestPriority = possibleSpawns[0].Priority;
            possibleSpawns.RemoveAll(spawn => spawn.Priority > lowestPriority);
            var spawnChoice = possibleSpawns[ResourceSystem.Random.NextInt(possibleSpawns.Count)];
            return spawnChoice.Use();
        }

        public void ResetSpawns()
        {
            foreach (var spawn in Spawns)
            {
                spawn.Reset();
            }
        }

        public void InitMatrix()
        {
            // Initialize all nodes
            foreach (var node in Nodes)
            {
                node.Init?.Invoke(node.State);
                ActiveNodes.Add(node);
            }
            foreach (var obj in Objects.Values.Where(obj => obj.GetNode != null))
            {
                ActiveNodes.Add(obj.GetNode(obj.InstanceController));
            }

            // Add all ports
            for (var i = 0; i < ActiveNodes.Count; i++)
            {
                var node = ActiveNodes[i];
                foreach (var port in node.GetPorts())
                {
                    ActivePorts[(i, port.Id)] = port;
                }
            }

            // Create link buffer
            foreach (var link in Links)
            {
                LinkBuffer[(link.ToNode, link.ToPort)] = false;
            }

            MatrixActive = true;
        }

        public void UpdateMatrix()
        {
            if (!MatrixActive) return;

            // Update all nodes
            foreach (var node in Nodes)
            {
                node.Update?.Invoke(node.State);
            }

            // Copy values between linked ports
            foreach (var link in Links)
            {
                var fromNode = ActiveNodes[link.FromNode];
                var fromPort = ActivePorts[(link.FromNode, link.FromPort)];
                var toPort = ActivePorts[(link.ToNode, link.ToPort)];
                Debug.Log($"{fromNode}\t{link}\t{ActiveNodes[link.ToNode]}");
                LinkBuffer[(link.ToNode, link.ToPort)] =
                    BsPort.Convert(fromPort.GetValue(fromNode.State), fromPort.Type, toPort.Type);
            }
            foreach (var link in Links)
            {
                var toNode = ActiveNodes[link.ToNode];
                var toPort = ActivePorts[(link.ToNode, link.ToPort)];
                toPort.SetValue(toNode.State, LinkBuffer[(link.ToNode, link.ToPort)]);
            }
        }

        public void ClearMatrix()
        {
            ActiveNodes.Clear();
            ActivePorts.Clear();
            LinkBuffer.Clear();

            MatrixActive = false;
        }

        public LcsDocument _ToLcs()
        {
            var result = new List<LcsLine>
            {
                new('!', Title, Author, PlayArea.ToLcs(), BackgroundColor.ToLcs(), LightingColor.ToLcs(),
                    GravityDirection, GravityStrength, AirResistance, PlayerHealth, AllowDummies)
            };
            result.AddRange(Spawns.Select(LcsLine.Serialize));
            result.AddRange(Objects.Values.Select(LcsLine.Serialize));
            result.AddRange(Nodes.Select(LcsLine.Serialize));
            result.AddRange(Links.Select(LcsLine.Serialize));
            return new LcsDocument(1, new[] { "!@#$%" }, result.ToArray());
        }

        public void _FromLcs(LcsDocument document)
        {
            var metadata = document.Lines[0];
            Title = metadata.Get<string>(0);
            Author = metadata.Get<string>(1);
            PlayArea = RectExt.FromLcs(metadata.Get(2));
            BackgroundColor = ColorExt.FromLcs(metadata.Get(3));
            LightingColor = ColorExt.FromLcs(metadata.Get(4));
            GravityDirection = metadata.Get<byte>(5);
            GravityStrength = metadata.Get<float>(6);
            AirResistance = metadata.Get<float>(7);
            PlayerHealth = metadata.Get<float>(8);
            AllowDummies = metadata.Get<bool>(9);
            foreach (var line in document.Lines[1..])
            {
                switch (line.Prefix)
                {
                    case '@':
                        Spawns.Add(LcsLine.Deserialize<BsSpawn>(line));
                        break;
                    case '#':
                        Objects.Add(LcsLine.Deserialize<BsObject>(line));
                        break;
                    case '$':
                        Nodes.Add(LcsLine.Deserialize<BsNode>(line));
                        break;
                    case '%':
                        Links.Add(LcsLine.Deserialize<BsLink>(line));
                        break;
                }
            }
        }

        public override string ToString()
        {
            return $"MAP: {Title} by {Author}";
        }
    }
}
