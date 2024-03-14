using UnityEngine;

namespace Brutalsky.Object
{
    public class BsTransform
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public BsTransform(Vector2 position, float rotation = 0f)
        {
            Position = position;
            Rotation = rotation;
        }

        public BsTransform(float x, float y, float rotation = 0f)
        {
            Position = new Vector2(x, y);
            Rotation = rotation;
        }

        public BsTransform(float rotation)
        {
            Rotation = rotation;
        }

        public BsTransform()
        {
        }

        public static BsTransform Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new BsTransform(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
        }

        public override string ToString()
        {
            return $"{Position.x} {Position.y} {Rotation}";
        }
    }
}
