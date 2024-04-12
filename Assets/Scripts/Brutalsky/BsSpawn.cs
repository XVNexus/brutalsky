using UnityEngine;
using Utils.Lcs;

namespace Brutalsky
{
    public class BsSpawn
    {
        public Vector2 Position { get; set; }
        public int Priority { get; set; }
        public int Usages { get; private set; }

        public BsSpawn(Vector2 position, int priority = 0)
        {
            Position = position;
            Priority = priority;
        }

        public BsSpawn()
        {
        }

        public LcsLine ToLcs()
        {
            return new LcsLine
            (
                '$',
                new[] { LcsParser.Stringify(Position) },
                new[] { LcsParser.Stringify(Priority) }
            );
        }

        public void FromLcs(LcsLine line)
        {
            Position = LcsParser.ParseVector2(line.Header[0]);
            Priority = LcsParser.ParseInt(line.Properties[0]);
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
    }
}
