using UnityEngine;

namespace Brutalsky.Property
{
    public class BsTransform : BsProperty
    {
        public Vector2 position { get; set; }
        public float rotation { get; set; }

        public BsTransform(Vector2 position, float rotation = 0f)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public BsTransform(float x, float y, float rotation = 0f)
        {
            position = new Vector2(x, y);
            this.rotation = rotation;
        }

        public BsTransform(float rotation)
        {
            this.rotation = rotation;
        }

        public BsTransform()
        {
        }

        public override void Parse(string raw)
        {
            var parts = raw.Split(' ');
            position = new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
            rotation = float.Parse(parts[2]);
        }

        public override string Stringify()
        {
            return $"{position.x} {position.y} {rotation}";
        }
    }
}
