using UnityEngine;
using Utils.Ext;

namespace Brutalsky.Shape
{
    public class BsPathLine : BsPathNode
    {
        public override int detailLevel => 1;

        public BsPathLine(Vector2 point)
        {
            endpoint = point;
        }

        public BsPathLine(float x, float y)
        {
            endpoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return MathfExt.Lerp(startpoint, endpoint, t);
        }
    }
}
