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
        public Dictionary<string, SrzShape> sh { get; set; } = new();
        public Dictionary<string, SrzJoint> jn { get; set; } = new();
        public Dictionary<string, SrzPool> pl { get; set; } = new();

        public static SrzMap Simplify(BsMap map)
        {
            var result = new SrzMap
            {
                tt = map.Title,
                at = map.Author,
                sz = Vector2Ext.ToString(map.Size),
                lg = map.Lighting.ToString()
            };
            foreach (var spawn in map.Spawns)
            {
                result.sp.Add(SrzSpawn.Simplify(spawn));
            }
            foreach (var shape in map.Shapes.Values)
            {
                result.sh[shape.Id] = SrzShape.Simplify(shape);
            }
            foreach (var joint in map.Joints.Values)
            {
                result.jn[joint.Id] = SrzJoint.Simplify(joint);
            }
            foreach (var pool in map.Pools.Values)
            {
                result.pl[pool.Id] = SrzPool.Simplify(pool);
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
                result.AddSpawn(spawn.Expand());
            }
            foreach (var shapeId in sh.Keys)
            {
                result.AddObject(sh[shapeId].Expand(shapeId));
            }
            foreach (var jointId in jn.Keys)
            {
                result.AddObject(jn[jointId].Expand(jointId));
            }
            foreach (var poolId in pl.Keys)
            {
                result.AddObject(pl[poolId].Expand(poolId));
            }
            return result;
        }
    }
}
