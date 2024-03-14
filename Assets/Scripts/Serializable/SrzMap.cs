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
                title = map.Title,
                author = map.Author,
                size = $"{map.Size.x} {map.Size.y}",
                lighting = map.Lighting.ToString()
            };
            foreach (var spawn in map.Spawns.Values)
            {
                result.spawns.Add(SrzSpawn.Simplify(spawn));
            }
            foreach (var shape in map.Shapes.Values)
            {
                result.shapes.Add(SrzShape.Simplify(shape));
            }
            foreach (var pool in map.Pools.Values)
            {
                result.pools.Add(SrzPool.Simplify(pool));
            }
            foreach (var joint in map.Joints.Values)
            {
                result.joints.Add(SrzJoint.Simplify(joint));
            }
            return result;
        }

        public BsMap Expand()
        {
            var result = new BsMap(title, author)
            {
                Size = Vector2Ext.Parse(size),
                Lighting = BsColor.Parse(lighting)
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
