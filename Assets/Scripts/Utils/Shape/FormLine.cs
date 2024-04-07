using UnityEngine;
using Utils.Ext;

namespace Utils.Shape
{
    public class FormLine : FormNode
    {
        public override int DetailLevel => 1;

        public FormLine(Vector2 point)
        {
            EndPoint = point;
        }

        public FormLine(float x, float y)
        {
            EndPoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return MathfExt.Lerp(StartPoint, EndPoint, t);
        }
    }
}
