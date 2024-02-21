using System.Collections.Generic;
using System.Linq;
using Brutalsky.Object;
using Core;
using UnityEngine;
using Utils;

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

        public void Parse(string raw)
        {
            var lines = raw.Split('\n');
            var metadataParts = lines[0][4..].Split(" / ");
            title = Sanitizer.Desanitize(metadataParts[0]);
            author = Sanitizer.Desanitize(metadataParts[1]);
            spawns.Clear();
            shapes.Clear();
            pools.Clear();
            joints.Clear();
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var lineData = Sanitizer.String2Array(line[4..]);
                switch (line[0])
                {
                    case 'X':
                        var spawn = new BsSpawn();
                        spawn.Parse(lineData);
                        spawns.Add(spawn);
                        break;
                    case 'S':
                        var shape = new BsShape();
                        shape.Parse(lineData);
                        shapes.Add(shape);
                        break;
                    case 'P':
                        var pool = new BsPool();
                        pool.Parse(lineData);
                        pools.Add(pool);
                        break;
                    case 'J':
                        var joint = new BsJoint();
                        joint.Parse(lineData);
                        joints.Add(joint);
                        break;
                }
            }
        }

        public string Stringify()
        {
            var result = $"# : {Sanitizer.Sanitize(title)} / {Sanitizer.Sanitize(author)}\n";
            result += spawns.Aggregate("", (current, spawn)
                => current + $"X : {Sanitizer.Array2String(spawn.Stringify())}\n");
            result += shapes.Aggregate("", (current, shape)
                => current + $"S : {Sanitizer.Array2String(shape.Stringify())}\n");
            result += pools.Aggregate("", (current, pool)
                => current + $"P : {Sanitizer.Array2String(pool.Stringify())}\n");
            result += joints.Aggregate("", (current, joint)
                => current + $"J : {Sanitizer.Array2String(joint.Stringify())}\n");
            return result;
        }
    }
}
