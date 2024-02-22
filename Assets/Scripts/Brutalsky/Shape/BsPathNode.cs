using UnityEngine;

namespace Brutalsky.Shape
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
}
