using UnityEngine;

namespace Brutalsky.Shape
{
    public abstract class BsPathNode
    {
        public BsPathNode Previous
        {
            get => _previous;
            set {
                _previous = value;
                value._next = this;
            }
        }
        private BsPathNode _previous;
        public BsPathNode Next
        {
            get => _next;
            set {
                _next = value;
                value._previous = this;
            }
        }
        private BsPathNode _next;

        public Vector2 StartPoint => Previous.EndPoint;
        public Vector2 EndPoint { get; set; }
        public abstract int DetailLevel { get; }

        public abstract Vector2 SamplePoint(float t);
    }
}
