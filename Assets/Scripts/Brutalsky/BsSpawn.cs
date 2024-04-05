using Serializable;
using UnityEngine;
using Utils.Ext;

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
            return new SrzSpawn(Vector2Ext.Stringify(Position), Priority);
        }

        public void FromSrz(SrzSpawn srzSpawn)
        {
            Position = Vector2Ext.Parse(srzSpawn.ps);
            Priority = srzSpawn.pr;
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
