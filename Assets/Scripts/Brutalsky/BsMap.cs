using System;
using System.Collections.Generic;
using System.Linq;
using Brutalsky.Base;
using Brutalsky.Logic;
using Brutalsky.Map;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Lcs;
using Color = UnityEngine.Color;

namespace Brutalsky
{
    public class BsMap
    {
        public uint Id => MapSystem.GenerateId(Title, Author);
        public string Title { get; set; }
        public string Author { get; set; }
        public Rect PlayArea
        {
            get => _playArea;
            set
            {
                if (value.width > MapSystem._.maxMapSize || value.height > MapSystem._.maxMapSize)
                    throw Errors.OversizedMap(value.size, MapSystem._.maxMapSize);
                _playArea = value;
            }
        }

        private Rect _playArea;
        public Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value.StripAlpha(); }
        private Color _backgroundColor;
        public Color LightingTint
        {
            get => LightingColor.MergeAlpha();
            set => LightingColor = new Color(value.r, value.g, value.b, LightingColor.a);
        }
        public float LightingIntensity
        {
            get => LightingColor.a;
            set => LightingColor = new Color(LightingColor.r, LightingColor.g, LightingColor.b, value);
        }
        public Color LightingColor;
        public Direction GravityDirection { get; set; }
        public float GravityStrength { get; set; }
        public float PlayerHealth { get; set; }
        public bool AllowDummies { get; set; }

        public List<BsSpawn> Spawns { get; } = new();
        public Dictionary<(string, string), BsObject> Objects { get; } = new();
        public List<BsNode> Nodes { get; } = new();
        public Dictionary<BsPort, BsLink> Links { get; } = new();

        public BsMap(string title = "Untitled Map", string author = "Anonymous Marble")
        {
            Title = title;
            Author = author;
        }

        public BsMap()
        {
        }

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

        public BsSpawn GetSpawn(int index)
        {
            return ContainsSpawn(index) ? Spawns[index] : null;
        }

        public void AddSpawn(BsSpawn spawn)
        {
            Spawns.Add(spawn);
        }

        public bool RemoveSpawn(int index)
        {
            if (!ContainsSpawn(index)) return false;
            Spawns.RemoveAt(index);
            return true;
        }

        public bool ContainsSpawn(int index)
        {
            return index >= 0 && index < Spawns.Count;
        }

        public T GetObject<T>(string tag, string id) where T : BsObject
        {
            return ContainsObject(tag, id) ? (T)Objects[(tag, id)] : throw Errors.NoItemFound("object", $"{tag}:{id}");
        }

        public bool AddObject(BsObject obj)
        {
            if (ContainsObject(obj)) return false;
            Objects[(obj.Tag, obj.Id)] = obj;
            return true;
        }

        public bool RemoveObject(BsObject obj)
        {
            return RemoveObject(obj.Tag, obj.Id);
        }

        public bool RemoveObject(string tag, string id)
        {
            return Objects.Remove((tag, id));
        }

        public bool ContainsObject(BsObject obj)
        {
            return ContainsObject(obj.Tag, obj.Id);
        }

        public bool ContainsObject(string tag, string id)
        {
            return Objects.ContainsKey((tag, id));
        }

        [CanBeNull]
        public BsNode GetNode(int id)
        {
            return ContainsNode(id) ? Nodes[id] : null;
        }

        public void AddNode(BsNode node)
        {
            Nodes.Add(node);
        }

        public bool RemoveNode(int id)
        {
            if (!ContainsNode(id)) return false;
            Nodes.RemoveAt(id);
            return true;
        }

        public bool ContainsNode(int id)
        {
            return id >= 0 && id < Nodes.Count;
        }

        public BsLink GetLink(BsPort toPort)
        {
            return Links[toPort];
        }

        public bool AddLink(BsLink link)
        {
            if (ContainsLink(link)) return false;
            Links[link.ToPort] = link;
            return true;
        }

        public bool RemoveLink(BsLink link)
        {
            return RemoveLink(link.ToPort);
        }

        public bool RemoveLink(BsPort toPort)
        {
            if (!ContainsLink(toPort)) return false;
            Links.Remove(toPort);
            return true;
        }

        public bool ContainsLink(BsLink link)
        {
            return ContainsLink(link.ToPort);
        }

        public bool ContainsLink(BsPort toPort)
        {
            return Links.ContainsKey(toPort);
        }

        public LcsDocument ToLcs()
        {
            var lines = new List<LcsLine>
            {
                new('!', new[]
                {
                    new LcsProp(LcsType.String, Title),
                    new LcsProp(LcsType.String, Author),
                    new LcsProp(LcsType.Rect, PlayArea),
                    new LcsProp(LcsType.Color, BackgroundColor),
                    new LcsProp(LcsType.Color, LightingColor),
                    new LcsProp(LcsType.Direction, GravityDirection),
                    new LcsProp(LcsType.Float, GravityStrength),
                    new LcsProp(LcsType.Float, PlayerHealth),
                    new LcsProp(LcsType.Bool, AllowDummies)
                })
            };
            lines.AddRange(Spawns.Select(spawn => spawn.ToLcs()));
            lines.AddRange(Objects.Values.Select(obj => obj.ToLcs()));
            lines.AddRange(Nodes.Select(node => node.ToLcs()));
            lines.AddRange(Links.Values.Select(link => link.ToLcs()));
            return new LcsDocument(1, lines, new[] { "!$#%^", "@" });
        }

        public static BsMap FromLcs(LcsDocument document)
        {
            var result = new BsMap();
            if (document.Lines.Count == 0) throw Errors.EmptyLcsDocument();
            var metadata = document.Lines[0].Props;
            if (document.Lines[0].Prefix != '!') throw Errors.InvalidItem("map LCS metadata line", metadata);
            result.Title = (string)metadata[0].Value;
            result.Author = (string)metadata[1].Value;
            result.PlayArea = (Rect)metadata[2].Value;
            result.BackgroundColor = (Color)metadata[3].Value;
            result.LightingColor = (Color)metadata[4].Value;
            result.GravityDirection = (Direction)metadata[5].Value;
            result.GravityStrength = (float)metadata[6].Value;
            result.PlayerHealth = (float)metadata[7].Value;
            result.AllowDummies = (bool)metadata[8].Value;
            for (var i = 1; i < document.Lines.Count; i++)
            {
                var line = document.Lines[i];
                switch (line.Prefix)
                {
                    case '$':
                        result.AddSpawn(BsSpawn.FromLcs(line));
                        break;
                    case '#':
                        result.AddObject(BsObject.FromLcs(line));
                        break;
                    case '%':
                        result.AddNode(BsNode.FromLcs(line));
                        break;
                    case '^':
                        result.AddLink(BsLink.FromLcs(line));
                        break;
                    default:
                        throw Errors.InvalidItem("LCS line prefix", line.Prefix);
                }
            }
            return result;
        }
    }
}
