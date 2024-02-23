using System.Collections.Generic;
using System.Linq;
using Brutalsky.Object;
using Core;
using UnityEngine;

namespace Brutalsky
{
    public class BsMap
    {
        public string title { get; set; }
        public string author { get; set; }
        public Vector2 size { get; set; }
        public BsColor lighting { get; set; }
        public Dictionary<string, BsSpawn> spawns { get; } = new();
        public Dictionary<string, BsShape> shapes { get; } = new();
        public Dictionary<string, BsPool> pools { get; } = new();
        public Dictionary<string, BsJoint> joints { get; } = new();

        public BsMap(string title = "Untitled Map", string author = "Anonymous Marble")
        {
            this.title = title;
            this.author = author;
        }

        public BsSpawn GetSpawn(string id)
        {
            return ContainsSpawn(id) ? spawns[id] : null;
        }

        public BsShape GetShape(string id)
        {
            return ContainsShape(id) ? shapes[id] : null;
        }

        public BsPool GetPool(string id)
        {
            return ContainsPool(id) ? pools[id] : null;
        }

        public BsJoint GetJoint(string id)
        {
            return ContainsJoint(id) ? joints[id] : null;
        }

        public bool Add(BsSpawn spawn)
        {
            if (Contains(spawn)) return false;
            spawns[spawn.id] = spawn;
            return true;
        }

        public bool Add(BsShape shape)
        {
            if (Contains(shape)) return false;
            shapes[shape.id] = shape;
            return true;
        }

        public bool Add(BsPool pool)
        {
            if (Contains(pool)) return false;
            pools[pool.id] = pool;
            return true;
        }

        public bool Add(BsJoint joint)
        {
            if (Contains(joint)) return false;
            joints[joint.id] = joint;
            return true;
        }

        public bool Remove(BsObject obj)
        {
            return Remove(obj.id);
        }

        public bool Remove(string id)
        {
            return spawns.Remove(id) || shapes.Remove(id) || pools.Remove(id) || joints.Remove(id);
        }

        public bool Contains(BsObject obj)
        {
            return Contains(obj.id);
        }

        public bool Contains(string id)
        {
            return ContainsSpawn(id) || ContainsShape(id) || ContainsPool(id) || ContainsJoint(id);
        }

        public bool ContainsSpawn(string id)
        {
            return spawns.ContainsKey(id);
        }

        public bool ContainsShape(string id)
        {
            return shapes.ContainsKey(id);
        }

        public bool ContainsPool(string id)
        {
            return pools.ContainsKey(id);
        }

        public bool ContainsJoint(string id)
        {
            return joints.ContainsKey(id);
        }

        public Vector2 SelectSpawn()
        {
            // TODO: MAKE THIS TAKE PRIORITY INTO ACCOUNT
            var leastUsages = spawns.Values.Select(spawn => spawn.usages).Prepend(int.MaxValue).Min();
            var possibleSpawns = spawns.Values.Where(spawn => spawn.usages == leastUsages).ToList();
            possibleSpawns.Sort((a, b) => a.priority - b.priority);
            var spawnChoice = possibleSpawns[EventSystem.random.NextInt(possibleSpawns.Count)];
            return spawnChoice.Use();
        }

        public void ResetSpawns()
        {
            foreach (var spawn in spawns.Values)
            {
                spawn.Reset();
            }
        }
    }
}
