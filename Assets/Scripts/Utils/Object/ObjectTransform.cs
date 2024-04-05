using UnityEngine;

namespace Utils.Object
{
    public class ObjectTransform
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public ObjectTransform(Vector2 position, float rotation = 0f)
        {
            Position = position;
            Rotation = rotation;
        }

        public ObjectTransform(float x, float y, float rotation = 0f)
        {
            Position = new Vector2(x, y);
            Rotation = rotation;
        }

        public ObjectTransform(float rotation)
        {
            Rotation = rotation;
        }

        public ObjectTransform()
        {
        }

        public static ObjectTransform Parse(string raw)
        {
            var parts = raw.Split(' ');
            return new ObjectTransform(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
        }

        public override string ToString()
        {
            return $"{Position.x} {Position.y} {Rotation}";
        }
    }
}
