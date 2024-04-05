using UnityEngine;
using Utils.Ext;

namespace Utils.Path
{
    public class PathLine : PathNode
    {
        public override int DetailLevel => 1;

        public PathLine(Vector2 point)
        {
            EndPoint = point;
        }

        public PathLine(float x, float y)
        {
            EndPoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return MathfExt.Lerp(StartPoint, EndPoint, t);
        }
    }
}
