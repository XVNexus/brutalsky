using System.Collections.Generic;
using System.Linq;
using Brutalsky.Base;
using Core;
using Serializable;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using Utils.Ext;
using Utils.Object;
using YamlDotNet.Serialization;

namespace Brutalsky
{
    public class BsMap
    {
        public uint Id { get; private set; }
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
            Id = BsUtils.GenerateId(title, author);
        }

        public BsMap()
        {
        }

        public SrzMap ToSrz()
        {
            var srzSpawns = new List<SrzSpawn>();
            foreach (var spawn in Spawns)
            {
                srzSpawns.Add(spawn.ToSrz());
            }
            var srzObjects = new List<SrzObject>();
            foreach (var obj in Objects.Values)
            {
                srzObjects.Add(obj.ToSrz());
            }
            return new SrzMap(Title, Author, Vector2Ext.Stringify(Size), Lighting.ToString(), srzSpawns, srzObjects);
        }

        public void FromSrz(SrzMap srzMap)
        {
            Title = srzMap.tt;
            Author = srzMap.at;
            Size = Vector2Ext.Parse(srzMap.sz);
            Lighting = ObjectColor.Parse(srzMap.lg);
            foreach (var srzSpawn in srzMap.sp)
            {
                var spawn = new BsSpawn();
                spawn.FromSrz(srzSpawn);
                AddSpawn(spawn);
            }
            foreach (var srzObject in srzMap.ob)
            {
                var objIdParts = BsUtils.SplitFullId(srzObject.id);
                var objTag = objIdParts[0];
                var objId = objIdParts[1];
                var obj = ResourceSystem._.GetTemplateObject(objTag);
                obj.FromSrz(objId, srzObject);
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
