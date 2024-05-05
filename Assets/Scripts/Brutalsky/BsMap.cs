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

namespace Brutalsky
{
    public class BsMap
    {
        public uint Id => MapSystem.GenerateId(Title, Author);
        public string Title { get; set; }
        public string Author { get; set; }
        public Vector2 PlayArea { get; set; }
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
        public List<BsSpawn> Spawns { get; } = new();
        public Dictionary<(string, string), BsObject> Objects { get; } = new();
        public List<BsNode> Nodes { get; } = new();
        public Dictionary<(int, int), BsLink> Links { get; } = new();

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

        [CanBeNull]
        public BsLink GetLink((int, int) toPort)
        {
            return ContainsLink(toPort) ? Links[toPort] : null;
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

        public bool RemoveLink((int, int) toPort)
        {
            if (!ContainsLink(toPort)) return false;
            Links.Remove(toPort);
            return true;
        }

        public bool ContainsLink(BsLink link)
        {
            return ContainsLink(link.ToPort);
        }

        public bool ContainsLink((int, int) toPort)
        {
            return Links.ContainsKey(toPort);
        }

        public string Stringify()
        {
            return ToLcs().Stringify();
        }

        public static BsMap Parse(string lcs)
        {
            return FromLcs(LcsDocument.Parse(lcs));
        }

        public LcsDocument ToLcs()
        {
            var lines = new List<LcsLine>
            {
                new('!', new[]
                {
                    Stringifier.Stringify(Title),
                    Stringifier.Stringify(Author),
                    Stringifier.Stringify(PlayArea),
                    Stringifier.Stringify(BackgroundColor),
                    Stringifier.Stringify(LightingColor),
                    Stringifier.Stringify(GravityDirection),
                    Stringifier.Stringify(GravityStrength),
                    Stringifier.Stringify(PlayerHealth)
                })
            };
            lines.AddRange(Spawns.Select(spawn => spawn.ToLcs()));
            lines.AddRange(Objects.Values.Select(obj => obj.ToLcs()));
            if (Nodes.Count > 0)
            {
                lines.AddRange(Nodes.Select(node => node.ToLcs()).ToArray());
                lines.AddRange(Links.Values.Select(link => link.ToLcs()).ToArray());
            }
            return new LcsDocument(1, lines, new[] { "!$#%^", "@" });
        }

        public static BsMap FromLcs(LcsDocument document)
        {
            var result = new BsMap();
            if (document.Lines.Count == 0)
            {
                throw Errors.EmptyLcsDocument();
            }
            var metadataLine = document.Lines[0];
            if (document.Lines[0].Prefix != '!')
            {
                throw Errors.InvalidItem("map LCS metadata line", metadataLine);
            }
            result.Title = Stringifier.ParseString(metadataLine.Properties[0]);
            result.Author = Stringifier.ParseString(metadataLine.Properties[1]);
            result.PlayArea = Stringifier.ParseVector2(metadataLine.Properties[2]);
            result.BackgroundColor = Stringifier.ParseColor(metadataLine.Properties[3]);
            result.LightingColor = Stringifier.ParseColor(metadataLine.Properties[4]);
            result.GravityDirection = Stringifier.ParseDirection(metadataLine.Properties[5]);
            result.GravityStrength = Stringifier.ParseFloat(metadataLine.Properties[6]);
            result.PlayerHealth = Stringifier.ParseFloat(metadataLine.Properties[7]);
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
