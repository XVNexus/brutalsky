using System.Collections.Generic;
using System.Linq;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;
using Color = UnityEngine.Color;

namespace Data
{
    public class BsMap
    {
        public const byte DirectionNone = 0;
        public const byte DirectionDown = 1;
        public const byte DirectionUp = 2;
        public const byte DirectionLeft = 3;
        public const byte DirectionRight = 4;

        public uint Id => MapSystem.GenerateId(Title, Author);
        public string Title { get; set; }
        public string Author { get; set; }
        public Rect PlayArea { get; set; } = new(-10f, -10f, 20f, 20f);
        public Color BackgroundColor { get; set; } = Color.white.MultiplyTint(.25f);
        public Color LightingColor = Color.white.SetAlpha(.8f);
        public byte GravityDirection { get; set; } = DirectionNone;
        public float GravityStrength { get; set; }
        public float AirResistance { get; set; }
        public float PlayerHealth { get; set; } = 100f;
        public bool AllowDummies { get; set; } = true;

        public List<BsSpawn> Spawns { get; } = new();
        public IdList<BsObject> Objects { get; } = new();

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
    }
}
