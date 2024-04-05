using System.Collections.Generic;
using System.Linq;
using Brutalsky.Base;
using Core;
using Serializable;
using UnityEngine;
using Utils.Object;
using YamlDotNet.Serialization;
using Random = Unity.Mathematics.Random;

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
        public Dictionary<string, BsShape> Shapes { get; } = new();
        public Dictionary<string, BsJoint> Joints { get; } = new();
        public Dictionary<string, BsPool> Pools { get; } = new();

        public BsMap(string title = "Untitled Map", string author = "Anonymous Marble")
        {
            Title = title;
            Author = author;
            Id = CalculateId(title, author);
        }

        public static uint CalculateId(string title, string author)
        {
            var idSeed = author.Aggregate<char, uint>(1, (current, letter) => current * letter)
                         + title.Aggregate<char, uint>(0, (current, letter) => current + letter);
            return Random.CreateFromIndex(idSeed).NextUInt();
        }

        public Vector2 SelectSpawn()
        {
            var leastUsages = Spawns.Select(spawn => spawn.Usages).Prepend(int.MaxValue).Min();
            var possibleSpawns = Spawns.Where(spawn => spawn.Usages == leastUsages).ToList();
            possibleSpawns.Sort((a, b) => a.Priority - b.Priority);
            var lowestPriority = possibleSpawns[0].Priority;
            possibleSpawns.RemoveAll(spawn => spawn.Priority > lowestPriority);
            var spawnChoice = possibleSpawns[EventSystem.Random.NextInt(possibleSpawns.Count)];
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

        public BsShape GetShape(string id)
        {
            return ContainsShape(id) ? Shapes[id] : null;
        }

        public BsJoint GetJoint(string id)
        {
            return ContainsJoint(id) ? Joints[id] : null;
        }

        public BsPool GetPool(string id)
        {
            return ContainsPool(id) ? Pools[id] : null;
        }

        public bool AddObject(BsShape shape)
        {
            if (ContainsObject(shape)) return false;
            Shapes[shape.Id] = shape;
            return true;
        }

        public bool AddObject(BsJoint joint)
        {
            if (ContainsObject(joint)) return false;
            Joints[joint.Id] = joint;
            return true;
        }

        public bool AddObject(BsPool pool)
        {
            if (ContainsObject(pool)) return false;
            Pools[pool.Id] = pool;
            return true;
        }

        public bool RemoveObject(BsObject obj)
        {
            return RemoveObject(obj.Id);
        }

        public bool RemoveObject(string id)
        {
            return Shapes.Remove(id) || Pools.Remove(id) || Joints.Remove(id);
        }

        public bool ContainsObject(BsObject obj)
        {
            return ContainsObject(obj.Id);
        }

        public bool ContainsObject(string id)
        {
            return ContainsShape(id) || ContainsPool(id) || ContainsJoint(id);
        }

        public bool ContainsShape(string id)
        {
            return Shapes.ContainsKey(id);
        }

        public bool ContainsJoint(string id)
        {
            return Joints.ContainsKey(id);
        }

        public bool ContainsPool(string id)
        {
            return Pools.ContainsKey(id);
        }

        public static BsMap Parse(string yaml)
        {
            return new Deserializer().Deserialize<SrzMap>(yaml).Expand();
        }

        public string Stringify()
        {
            return new Serializer().Serialize(SrzMap.Simplify(this));
        }
    }
}
