using UnityEngine;

namespace Brutalsky.Shape
{
    public class BsPathStart : BsPathNode
    {
        public override int DetailLevel => 0;

        public BsPathStart(Vector2 point)
        {
            EndPoint = point;
        }

        public BsPathStart(float x, float y)
        {
            EndPoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return EndPoint;
        }
    }
}
