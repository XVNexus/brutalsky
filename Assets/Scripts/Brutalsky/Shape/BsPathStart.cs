using UnityEngine;

namespace Brutalsky.Shape
{
    public class BsPathStart : BsPathNode
    {
        public override int detailLevel => 0;

        public BsPathStart(Vector2 point)
        {
            endpoint = point;
        }

        public BsPathStart(float x, float y)
        {
            endpoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return endpoint;
        }
    }
}
