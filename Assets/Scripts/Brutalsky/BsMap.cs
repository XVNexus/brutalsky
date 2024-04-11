using System.Collections.Generic;
using System.Linq;
using Brutalsky.Base;
using Core;
using UnityEngine;
using Utils;
using Utils.Constants;
using Utils.Lcs;
using Utils.Object;
using YamlDotNet.Serialization;

namespace Brutalsky
{
    public class BsMap
    {
        public uint Id => BsUtils.GenerateId(Title, Author);
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

        public LcsDocument ToLcs()
        {
            var lines = new List<LcsLine>
            {
                new(
                    '!',
                    new[] { SrzUtils.Stringify(Title), SrzUtils.ParseString(Author) },
                    new[] { SrzUtils.Stringify(Size), SrzUtils.Stringify(Lighting) }
                )
            };
            lines.AddRange(Spawns.Select(spawn => spawn.ToLcs()));
            lines.AddRange(Objects.Values.Select(obj => obj.ToLcs()));
            return new LcsDocument(lines);
        }

        public void FromLcs(LcsDocument document)
        {
            if (document.Lines.Count == 0)
            {
                throw Errors.EmptyLcs();
            }
            var metadataLine = document.Lines[0];
            if (document.Lines[0].Prefix != '!')
            {
                throw Errors.InvalidLcs(metadataLine, 0);
            }
            Title = SrzUtils.ParseString(metadataLine.Header[0]);
            Author = SrzUtils.ParseString(metadataLine.Header[1]);
            Size = SrzUtils.ParseVector2(metadataLine.Properties[0]);
            Lighting = SrzUtils.ParseColor(metadataLine.Properties[1]);
            for (var i = 1; i < document.Lines.Count; i++)
            {
                var line = document.Lines[i];
                switch (line.Prefix)
                {
                    case '$':
                        break;
                    case '#':
                        break;
                    case '@':
                        break;
                    default:
                        throw Errors.InvalidLcs(line, i);
                }
            }
            _FromSrz(line.Properties);
            foreach (var child in line.Children)
            {
                var addon = ResourceSystem._.GetTemplateAddon(child.Header[0]);
                addon.FromLcs(child);
                Addons.Add(addon);
            }
        }

        public void FromSrz(SrzMap srzMap)
        {
            var parts = srzMap.Properties;
            Title = SrzUtils.ParseString(parts[0]);
            Author = SrzUtils.ParseString(parts[1]);
            Size = SrzUtils.ParseVector2(parts[2]);
            Lighting = SrzUtils.ParseColor(parts[3]);
            foreach (var srzSpawn in srzMap.Spawns)
            {
                var spawn = new BsSpawn();
                spawn.FromLcs(srzSpawn);
                AddSpawn(spawn);
            }
            foreach (var srzObject in srzMap.Objects)
            {
                var obj = ResourceSystem._.GetTemplateObject(srzObject.Tag);
                obj.FromSrz(srzObject.Id, srzObject);
                AddObject(obj);
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

        public T GetObject<T>(string tag, string id) where T : BsObject
        {
            return ContainsObject(tag, id) ? (T)Objects[BsUtils.GenerateFullId(tag, id)] : null;
        }

        public bool AddObject(BsObject obj)
        {
            if (ContainsObject(obj)) return false;
            Objects[BsUtils.GenerateFullId(obj.Tag, obj.Id)] = obj;
            return true;
        }

        public bool RemoveObject(BsObject obj)
        {
            return RemoveObject(obj.Tag, obj.Id);
        }

        public bool RemoveObject(string tag, string id)
        {
            return Objects.Remove(BsUtils.GenerateFullId(tag, id));
        }

        public bool ContainsObject(BsObject obj)
        {
            return ContainsObject(obj.Tag, obj.Id);
        }

        public bool ContainsObject(string tag, string id)
        {
            return Objects.ContainsKey(BsUtils.GenerateFullId(tag, id));
        }

        public static BsMap Parse(string yaml)
        {
            var result = new BsMap();
            result.FromSrz(new Deserializer().Deserialize<SrzMap>(yaml));
            return result;
        }

        public string Stringify()
        {
            return new Serializer().Serialize(ToSrz());
        }
    }
}
