using UnityEngine;

namespace Brutalsky
{
    public class BsTransform
    {
        public Vector2 position { get; set; }
        public float rotation { get; set; }

        public BsTransform(Vector2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        public BsTransform(float x, float y, float rotation)
        {
            position = new Vector2(x, y);
            this.rotation = rotation;
        }

        public BsTransform(Vector2 position)
        {
            this.position = position;
        }

        public BsTransform(float x, float y)
        {
            position = new Vector2(x, y);
        }

        public BsTransform(float rotation)
        {
            this.rotation = rotation;
        }

        public BsTransform()
        {
        }
    }
}
