using UnityEngine;

namespace Utils.Shape
{
    public class FormStart : FormNode
    {
        public override int DetailLevel => 0;

        public FormStart(Vector2 point)
        {
            EndPoint = point;
        }

        public FormStart(float x, float y)
        {
            EndPoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return EndPoint;
        }
    }
}
