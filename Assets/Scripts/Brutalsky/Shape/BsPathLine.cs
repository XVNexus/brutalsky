using UnityEngine;
using Utils.Ext;

namespace Brutalsky.Shape
{
    public class BsPathLine : BsPathNode
    {
        public override int DetailLevel => 1;

        public BsPathLine(Vector2 point)
        {
            EndPoint = point;
        }

        public BsPathLine(float x, float y)
        {
            EndPoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return MathfExt.Lerp(StartPoint, EndPoint, t);
        }
    }
}
