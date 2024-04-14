using System.Collections.Generic;
using System.Linq;
using Brutalsky.Base;
using Core;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky
{
    public class BsMap
    {
        public uint Id => MapSystem.GenerateId(Title, Author);
        public string Title { get; set; }
        public string Author { get; set; }
        public Vector2 Size { get; set; }
        public ObjectColor Lighting { get; set; }
        public List<BsSpawn> Spawns { get; } = new();
        public Dictionary<string, BsObject> Objects { get; } = new();

        public BsMap(string title = "Untitled Map", string author = "Anonymous Marble")
        {
            Title = title;
            Author = author;
        }

        public BsMap()
        {
        }

        public string Stringify()
        {
            return ToLcs().Stringify();
        }

        public static BsMap Parse(string lcs)
        {
            var result = new BsMap();
            result.FromLcs(LcsDocument.Parse(lcs));
            return result;
        }

        public LcsDocument ToLcs()
        {
            var lines = new List<LcsLine>
            {
                new(
                    '!',
                    new[] { LcsParser.Stringify(Title), LcsParser.Stringify(Author) },
                    new[] { LcsParser.Stringify(Size), LcsParser.Stringify(Lighting) }
                )
            };
            lines.AddRange(Spawns.Select(spawn => spawn.ToLcs()));
            lines.AddRange(Objects.Values.Select(obj => obj.ToLcs()));
            return new LcsDocument(lines, new[] { "!$#", "@" });
        }

        public void FromLcs(LcsDocument document)
        {
            if (document.Lines.Count == 0)
            {
                throw Errors.EmptyLcsDocument();
            }
            var metadataLine = document.Lines[0];
            if (document.Lines[0].Prefix != '!')
            {
                throw Errors.InvalidLcsLine(metadataLine, 0);
            }
            Title = LcsParser.ParseString(metadataLine.Header[0]);
            Author = LcsParser.ParseString(metadataLine.Header[1]);
            Size = LcsParser.ParseVector2(metadataLine.Properties[0]);
            Lighting = LcsParser.ParseColor(metadataLine.Properties[1]);
            for (var i = 1; i < document.Lines.Count; i++)
            {
                var line = document.Lines[i];
                switch (line.Prefix)
                {
                    case '$':
                        var spawn = new BsSpawn();
                        spawn.FromLcs(line);
                        AddSpawn(spawn);
                        break;
                    case '#':
                        var obj = ResourceSystem._.GetTemplateObject(LcsParser.ParseChar(line.Header[0]));
                        obj.FromLcs(line);
                        AddObject(obj);
                        break;
                    default:
                        throw Errors.InvalidLcsLine(line, i);
                }
            }
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

        public T GetObject<T>(char tag, string id) where T : BsObject
        {
            return ContainsObject(tag, id) ? (T)Objects[tag + id] : null;
        }

        public bool AddObject(BsObject obj)
        {
            if (ContainsObject(obj)) return false;
            Objects[obj.Tag + obj.Id] = obj;
            return true;
        }

        public bool RemoveObject(BsObject obj)
        {
            return RemoveObject(obj.Tag, obj.Id);
        }

        public bool RemoveObject(char tag, string id)
        {
            return Objects.Remove(tag + id);
        }

        public bool ContainsObject(BsObject obj)
        {
            return ContainsObject(obj.Tag, obj.Id);
        }

        public bool ContainsObject(char tag, string id)
        {
            return Objects.ContainsKey(tag + id);
        }
    }
}
