using UnityEngine;

namespace Brutalsky.Object
{
    public class BsTransform
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
    }
}
