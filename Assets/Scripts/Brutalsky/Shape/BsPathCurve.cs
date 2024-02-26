using UnityEngine;
using Utils.Ext;

namespace Brutalsky.Shape
{
    public class BsPathCurve : BsPathNode
    {
        public override int detailLevel => Mathf.CeilToInt(length * 2f * Mathf.PI);
        public Vector2 handle { get; set; }

        private static readonly float HandleMultiplier = Mathf.Pow(1f / 6f, 1f / 3f);
        private Vector2 p0 => startpoint;
        private Vector2 p1 => MathfExt.Lerp(startpoint, handle, HandleMultiplier);
        private Vector2 p2 => MathfExt.Lerp(endpoint, handle, HandleMultiplier);
        private Vector2 p3 => endpoint;
        private float length => ((p3 - p0).magnitude + (p1 - p0).magnitude + (p2 - p1).magnitude + (p3 - p2).magnitude) / 2f;

        public BsPathCurve(Vector2 handle, Vector2 point)
        {
            this.handle = handle;
            endpoint = point;
        }

        public BsPathCurve(float a, float b, float x, float y)
        {
            handle = new Vector2(a, b);
            endpoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            var p01 = MathfExt.Lerp(p0, p1, t);
            var p12 = MathfExt.Lerp(p1, p2, t);
            var p23 = MathfExt.Lerp(p2, p3, t);
            var p012 = MathfExt.Lerp(p01, p12, t);
            var p123 = MathfExt.Lerp(p12, p23, t);
            return MathfExt.Lerp(p012, p123, t);
        }
    }
}
