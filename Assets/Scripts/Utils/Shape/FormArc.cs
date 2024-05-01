using UnityEngine;
using Utils.Ext;

namespace Utils.Shape
{
    public class FormArc : FormNode
    {
        public override int DetailLevel => Mathf.CeilToInt(Length * 2f * Mathf.PI);
        public Vector2 Handle { get; set; }

        private static readonly float HandleMultiplier = Mathf.Pow(1f / 6f, 1f / 3f);
        private Vector2 P0 => StartPoint;
        private Vector2 P1 => MathfExt.Lerp(StartPoint, Handle, HandleMultiplier);
        private Vector2 P2 => MathfExt.Lerp(EndPoint, Handle, HandleMultiplier);
        private Vector2 P3 => EndPoint;
        private float Length => ((P3 - P0).magnitude + (P1 - P0).magnitude + (P2 - P1).magnitude + (P3 - P2).magnitude) * .5f;

        public FormArc(Vector2 handle, Vector2 point)
        {
            Handle = handle;
            EndPoint = point;
        }

        public FormArc(float a, float b, float x, float y)
        {
            Handle = new Vector2(a, b);
            EndPoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            var p01 = MathfExt.Lerp(P0, P1, t);
            var p12 = MathfExt.Lerp(P1, P2, t);
            var p23 = MathfExt.Lerp(P2, P3, t);
            var p012 = MathfExt.Lerp(p01, p12, t);
            var p123 = MathfExt.Lerp(p12, p23, t);
            return MathfExt.Lerp(p012, p123, t);
        }
    }
}
