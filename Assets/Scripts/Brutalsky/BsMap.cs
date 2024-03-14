using System.Collections.Generic;
using System.Linq;
using Brutalsky.Object;
using Core;
using Serializable;
using UnityEngine;
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
        public BsColor Lighting { get; set; }
        public Dictionary<string, BsSpawn> Spawns { get; } = new();
        public Dictionary<string, BsShape> Shapes { get; } = new();
        public Dictionary<string, BsPool> Pools { get; } = new();
        public Dictionary<string, BsJoint> Joints { get; } = new();

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

        public BsSpawn GetSpawn(string id)
        {
            return ContainsSpawn(id) ? Spawns[id] : null;
        }

        public BsShape GetShape(string id)
        {
            return ContainsShape(id) ? Shapes[id] : null;
        }

        public BsPool GetPool(string id)
        {
            return ContainsPool(id) ? Pools[id] : null;
        }

        public BsJoint GetJoint(string id)
        {
            return ContainsJoint(id) ? Joints[id] : null;
        }

        public bool Add(BsSpawn spawn)
        {
            if (Contains(spawn)) return false;
            Spawns[spawn.Id] = spawn;
            return true;
        }

        public bool Add(BsShape shape)
        {
            if (Contains(shape)) return false;
            Shapes[shape.Id] = shape;
            return true;
        }

        public bool Add(BsPool pool)
        {
            if (Contains(pool)) return false;
            Pools[pool.Id] = pool;
            return true;
        }

        public bool Add(BsJoint joint)
        {
            if (Contains(joint)) return false;
            Joints[joint.Id] = joint;
            return true;
        }

        public bool Remove(BsObject obj)
        {
            return Remove(obj.Id);
        }

        public bool Remove(string id)
        {
            return Spawns.Remove(id) || Shapes.Remove(id) || Pools.Remove(id) || Joints.Remove(id);
        }

        public bool Contains(BsObject obj)
        {
            return Contains(obj.Id);
        }

        public bool Contains(string id)
        {
            return ContainsSpawn(id) || ContainsShape(id) || ContainsPool(id) || ContainsJoint(id);
        }

        public bool ContainsSpawn(string id)
        {
            return Spawns.ContainsKey(id);
        }

        public bool ContainsShape(string id)
        {
            return Shapes.ContainsKey(id);
        }

        public bool ContainsPool(string id)
        {
            return Pools.ContainsKey(id);
        }

        public bool ContainsJoint(string id)
        {
            return Joints.ContainsKey(id);
        }

        public Vector2 SelectSpawn()
        {
            var leastUsages = Spawns.Values.Select(spawn => spawn.Usages).Prepend(int.MaxValue).Min();
            var possibleSpawns = Spawns.Values.Where(spawn => spawn.Usages == leastUsages).ToList();
            possibleSpawns.Sort((a, b) => a.Priority - b.Priority);
            var lowestPriority = possibleSpawns[0].Priority;
            possibleSpawns.RemoveAll(spawn => spawn.Priority > lowestPriority);
            var spawnChoice = possibleSpawns[EventSystem.Random.NextInt(possibleSpawns.Count)];
            return spawnChoice.Use();
        }

        public void ResetSpawns()
        {
            foreach (var spawn in Spawns.Values)
            {
                spawn.Reset();
            }
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
