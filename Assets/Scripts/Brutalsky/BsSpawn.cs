using Serializable;
using UnityEngine;
using Utils;

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

        public SrzSpawn ToSrz()
        {
            return new SrzSpawn(SrzUtils.CompressFields(new[]
            {
                SrzUtils.Stringify(Position.x),
                SrzUtils.Stringify(Position.y),
                SrzUtils.Stringify(Priority)
            }));
        }

        public void FromSrz(SrzSpawn srzSpawn)
        {
            var parts = SrzUtils.ExpandFields(srzSpawn.pr);
            Position = new Vector2(SrzUtils.ParseFloat(parts[0]), SrzUtils.ParseFloat(parts[1]));
            Priority = SrzUtils.ParseInt(parts[2]);
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
