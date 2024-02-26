using System.Collections.Generic;
using Brutalsky;
using Brutalsky.Object;
using Utils.Ext;

namespace Serializable
{
    public class SrzMap
    {
        public string title { get; set; }
        public string author { get; set; }
        public string size { get; set; }
        public string lighting { get; set; }
        public List<SrzSpawn> spawns { get; set; } = new();
        public List<SrzShape> shapes { get; set; } = new();
        public List<SrzPool> pools { get; set; } = new();
        public List<SrzJoint> joints { get; set; } = new();

        public static SrzMap Simplify(BsMap map)
        {
            var result = new SrzMap
            {
                title = map.title,
                author = map.author,
                size = $"{map.size.x} {map.size.y}",
                lighting = map.lighting.ToString()
            };
            foreach (var spawn in map.spawns.Values)
            {
                result.spawns.Add(SrzSpawn.Simplify(spawn));
            }
            foreach (var shape in map.shapes.Values)
            {
                result.shapes.Add(SrzShape.Simplify(shape));
            }
            foreach (var pool in map.pools.Values)
            {
                result.pools.Add(SrzPool.Simplify(pool));
            }
            foreach (var joint in map.joints.Values)
            {
                result.joints.Add(SrzJoint.Simplify(joint));
            }
            return result;
        }

        public BsMap Expand()
        {
            var result = new BsMap
            {
                title = title,
                author = author,
                size = Vector2Ext.Parse(size),
                lighting = BsColor.Parse(lighting)
            };
            foreach (var spawn in spawns)
            {
                result.Add(spawn.Expand());
            }
            foreach (var shape in shapes)
            {
                result.Add(shape.Expand());
            }
            foreach (var pool in pools)
            {
                result.Add(pool.Expand());
            }
            foreach (var joint in joints)
            {
                result.Add(joint.Expand());
            }
            return result;
        }
    }
}
