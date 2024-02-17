using UnityEngine;
using Utils;

namespace Brutalsky.Property
{
    public abstract class BsPathNode
    {
        public BsPathNode previous
        {
            get => _previous;
            set {
                _previous = value;
                value._next = this;
            }
        }
        private BsPathNode _previous;
        public BsPathNode next
        {
            get => _next;
            set {
                _next = value;
                value._previous = this;
            }
        }
        private BsPathNode _next;

        public Vector2 startpoint => previous.endpoint;
        public Vector2 endpoint { get; set; }
        public abstract int detailLevel { get; }

        public abstract Vector2 SamplePoint(float t);
    }

    public class BsPathStart : BsPathNode
    {
        public override int detailLevel => 0;

        public BsPathStart(Vector2 point)
        {
            endpoint = point;
        }

        public BsPathStart(float x, float y)
        {
            endpoint = new Vector2(x, y);
        }

        public override Vector2 SamplePoint(float t)
        {
            return endpoint;
        }
    }

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
            return MathfExt.Lerp2(startpoint, endpoint, t);
        }
    }

    public class BsPathCurve : BsPathNode
    {
        public override int detailLevel => Mathf.CeilToInt(length * 2f * Mathf.PI);
        public Vector2 handle { get; set; }

        private static readonly float HandleMultiplier = Mathf.Pow(1f / 6f, 1f / 3f);
        private Vector2 p0 => startpoint;
        private Vector2 p1 => MathfExt.Lerp2(startpoint, handle, HandleMultiplier);
        private Vector2 p2 => MathfExt.Lerp2(endpoint, handle, HandleMultiplier);
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
            var p01 = MathfExt.Lerp2(p0, p1, t);
            var p12 = MathfExt.Lerp2(p1, p2, t);
            var p23 = MathfExt.Lerp2(p2, p3, t);
            var p012 = MathfExt.Lerp2(p01, p12, t);
            var p123 = MathfExt.Lerp2(p12, p23, t);
            return MathfExt.Lerp2(p012, p123, t);
        }
    }
}
