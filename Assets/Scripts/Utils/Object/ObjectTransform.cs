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

        public override string ToString()
        {
            return $"({Position.x}, {Position.y}, {Rotation})";
        }
    }
}
