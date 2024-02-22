using System.Collections.Generic;
using System.Linq;
using Brutalsky.Object;
using Core;
using UnityEngine;

namespace Brutalsky
{
    public class BsMap
    {
        public string title { get; set; } = "Untitled Map";
        public string author { get; set; } = "Anonymous Marble";
        public List<BsSpawn> spawns { get; } = new();
        public List<BsShape> shapes { get; } = new();
        public List<BsPool> pools { get; } = new();
        public List<BsJoint> joints { get; } = new();

        public BsMap(string title, string author)
        {
            this.title = title;
            this.author = author;
        }

        public BsMap()
        {
        }

        public Vector2 SelectSpawn()
        {
            var leastUsages = spawns.Select(spawn => spawn.usages).Prepend(int.MaxValue).Min();
            var possibleSpawns = spawns.Where(spawn => spawn.usages == leastUsages).ToList();
            var spawnChoice = possibleSpawns[EventSystem.random.NextInt(possibleSpawns.Count)];
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
