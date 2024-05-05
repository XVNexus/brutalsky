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
            return new LcsLine('$', new[] { Stringifier.Stringify(Position), Stringifier.Stringify(Priority) });
        }

        public static BsSpawn FromLcs(LcsLine line)
        {
            return new BsSpawn(Stringifier.ParseVector2(line.Properties[0]), Stringifier.ParseInt(line.Properties[1]));
        }
    }
}
