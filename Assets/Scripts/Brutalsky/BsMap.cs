using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Brutalsky
{
    public class BsMap
    {
        public List<BsSpawn> spawns = new();
        public List<BsShape> shapes = new();
        public List<BsPool> pools = new();
        public List<BsJoint> joints = new();

        private Random random = Random.CreateFromIndex(0);

        public Vector2 SelectSpawn()
        {
            var leastUsages = spawns.Select(spawn => spawn.usages).Prepend(int.MaxValue).Min();
            var possibleSpawns = spawns.Where(spawn => spawn.usages == leastUsages).ToList();
            var spawnChoice = possibleSpawns[random.NextInt(possibleSpawns.Count)];
            return spawnChoice.Use();
        }

        public void ResetSpawns()
        {
            foreach (var spawn in spawns)
            {
                spawn.Reset();
            }
        }
    }
}
