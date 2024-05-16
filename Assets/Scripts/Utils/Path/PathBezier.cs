using UnityEngine;
using Utils.Ext;

namespace Utils.Path
{
    public class PathBezier : PathNode
    {
        public override int DetailLevel => Mathf.CeilToInt(Length * 2f * Mathf.PI);
        public Vector2 Handle1 { get; set; }
        public Vector2 Handle2 { get; set; }

        private Vector2 P0 => StartPoint;
        private Vector2 P1 => Handle1;
        private Vector2 P2 => Handle2;
        private Vector2 P3 => EndPoint;
        private float Length => ((P3 - P0).magnitude + (P1 - P0).magnitude + (P2 - P1).magnitude + (P3 - P2).magnitude) * .5f;

        public PathBezier(Vector2 handle1, Vector2 handle2, Vector2 point)
        {
            Handle1 = handle1;
            Handle2 = handle2;
            EndPoint = point;
        }

        public PathBezier(float a, float b, float c, float d, float x, float y)
        {
            Handle1 = new Vector2(a, b);
            Handle2 = new Vector2(c, d);
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
