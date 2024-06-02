using System;
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
        public Dictionary<int, float[]> InputBuffer { get; } = new();
        public Dictionary<int, int> InputPorts { get; } = new();
        public Dictionary<int, float> OutputBuffer { get; } = new();

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
            // Add all explicit nodes and implicit nodes
            ActiveNodes.Clear();
            foreach (var node in Nodes)
            {
                node.Init?.Invoke();
                ActiveNodes.Add(node);
                // TODO: ADD COVER NODES FOR OBJECTS
            }

            // Set up link buffers
            var inputCounts = new Dictionary<int, int>();
            InputBuffer.Clear();
            InputPorts.Clear();
            OutputBuffer.Clear();
            for (var i = 0; i < Links.Count; i++)
            {
                var link = Links[i];
                if (!inputCounts.TryAdd(link.To, 0))
                {
                    inputCounts[link.To]++;
                }
                InputPorts[i] = inputCounts[link.To];
                OutputBuffer.TryAdd(link.From, 0f);
            }
            foreach (var key in inputCounts.Keys)
            {
                InputBuffer[key] = new float[inputCounts[key] + 1];
            }
        }

        public void CalcMatrix()
        {
            // Update the input buffers with values from last frame's output buffer
            for (var i = 0; i < Links.Count; i++)
            {
                var link = Links[i];
                InputBuffer[link.To][InputPorts[i]] = OutputBuffer[link.From];
            }

            // Recalculate node outputs based on the input buffer and update the output buffer with the new values
            foreach (var link in Links)
            {
                OutputBuffer[link.To] = ActiveNodes[link.To].Calc(InputBuffer[link.To]);
            }
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
            return new LcsDocument(1, new[] { "!$#%^" }, result.ToArray());
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
                    case '$':
                        Spawns.Add(LcsLine.Deserialize<BsSpawn>(line));
                        break;
                    case '#':
                        Objects.Add(LcsLine.Deserialize<BsObject>(line));
                        break;
                    case '%':
                        Nodes.Add(LcsLine.Deserialize<BsNode>(line));
                        break;
                    case '^':
                        Links.Add(LcsLine.Deserialize<BsLink>(line));
                        break;
                }
            }
        }
    }
}
