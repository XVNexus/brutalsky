using UnityEngine;
using Utils.Lcs;

namespace Data.Map
{
    public class BsSpawn
    {
        public Vector2 Position { get; set; }
        public int Priority { get; set; }
        public int Usages { get; private set; }

        public BsSpawn(Vector2 position, int priority)
        {
            Position = position;
            Priority = priority;
        }

        public BsSpawn()
        {
        }

        public Vector2 Use()
        {
            Usages++;
            return Position;
        }

        public void Reset()
        {
            Usages = 0;
        }

        public LcsLine ToLcs()
        {
            return new LcsLine('$', Position, Priority);
        }

        public static BsSpawn FromLcs(LcsLine line)
        {
            return new BsSpawn((Vector2)line.Props[0], (int)line.Props[1]);
        }

        public override string ToString()
        {
            return $"SPAWN :: {Position}";
        }
    }
}
