using UnityEngine;
using Utils.Lcs;

namespace Brutalsky.Map
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
            return new LcsLine('$', new[]
            {
                new LcsProp(Position),
                new LcsProp(Priority)
            });
        }

        public static BsSpawn FromLcs(LcsLine line)
        {
            return new BsSpawn((Vector2)line.Props[0].Value, (int)line.Props[1].Value);
        }

        public override string ToString()
        {
            return $"SPAWN :: {Position}";
        }
    }
}
