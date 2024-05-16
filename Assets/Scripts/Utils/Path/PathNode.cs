using UnityEngine;

namespace Utils.Path
{
    public abstract class PathNode
    {
        public PathNode Previous
        {
            get => _previous;
            set {
                _previous = value;
                value._next = this;
            }
        }
        private PathNode _previous;
        public PathNode Next
        {
            get => _next;
            set {
                _next = value;
                value._previous = this;
            }
        }
        private PathNode _next;

        public Vector2 StartPoint => Previous.EndPoint;
        public Vector2 EndPoint { get; set; }
        public abstract int DetailLevel { get; }

        public abstract Vector2 SamplePoint(float t);
    }
}
