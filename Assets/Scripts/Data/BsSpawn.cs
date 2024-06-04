using Lcs;
using UnityEngine;

namespace Data
{
    public class BsSpawn : ILcsLine
    {
        public Vector2 Position { get; set; }
        public int Priority { get; set; }
        public int Usages { get; private set; }

        public BsSpawn(Vector2 position, int priority)
        {
            Position = position;
            Priority = priority;
        }

        public BsSpawn() { }

        public Vector2 Use()
        {
            Usages++;
            return Position;
        }

        public void Reset()
        {
            Usages = 0;
        }

        public LcsLine _ToLcs()
        {
            return new LcsLine('@', LcsInfo.Compress(Position.x, Position.y), Priority);
        }

        public void _FromLcs(LcsLine line)
        {
            Position = new Vector2(line.Get<float>(0, 0), line.Get<float>(0, 1));
            Priority = line.Get<int>(1);
        }

        public override string ToString()
        {
            return $"SPAWN: {Position.x} {Position.y} @ {Priority}";
        }
    }
}
