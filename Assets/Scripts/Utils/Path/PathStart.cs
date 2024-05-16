using UnityEngine;

namespace Utils.Path
{
    public class PathStart : PathNode
    {
        public override int DetailLevel => 0;

        public PathStart(Vector2 point)
        {
            EndPoint = point;
        }

        public PathStart(float x, float y)
        {
            EndPoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return EndPoint;
        }
    }
}
