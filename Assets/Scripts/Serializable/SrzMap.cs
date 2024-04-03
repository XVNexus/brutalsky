using System.Collections.Generic;
using Brutalsky;
using Brutalsky.Object;
using Utils.Ext;

namespace Serializable
{
    public class SrzMap
    {
        public string tt { get; set; }
        public string at { get; set; }
        public string sz { get; set; }
        public string lg { get; set; }
        public List<SrzSpawn> sp { get; set; } = new();
        public List<SrzShape> sh { get; set; } = new();
        public List<SrzPool> pl { get; set; } = new();
        public List<SrzJoint> jn { get; set; } = new();

        public static SrzMap Simplify(BsMap map)
        {
            var result = new SrzMap
            {
                tt = map.Title,
                at = map.Author,
                sz = $"{map.Size.x} {map.Size.y}",
                lg = map.Lighting.ToString()
            };
            foreach (var spawn in map.Spawns.Values)
            {
                result.sp.Add(SrzSpawn.Simplify(spawn));
            }
            foreach (var shape in map.Shapes.Values)
            {
                result.sh.Add(SrzShape.Simplify(shape));
            }
            foreach (var pool in map.Pools.Values)
            {
                result.pl.Add(SrzPool.Simplify(pool));
            }
            foreach (var joint in map.Joints.Values)
            {
                result.jn.Add(SrzJoint.Simplify(joint));
            }
            return result;
        }

        public BsMap Expand()
        {
            var result = new BsMap(tt, at)
            {
                Size = Vector2Ext.Parse(sz),
                Lighting = BsColor.Parse(lg)
            };
            foreach (var spawn in sp)
            {
                result.Add(spawn.Expand());
            }
            foreach (var shape in sh)
            {
                result.Add(shape.Expand());
            }
            foreach (var pool in pl)
            {
                result.Add(pool.Expand());
            }
            foreach (var joint in jn)
            {
                result.Add(joint.Expand());
            }
            return result;
        }
    }
}
