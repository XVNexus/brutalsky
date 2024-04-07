using UnityEngine;

namespace Utils.Shape
{
    public abstract class FormNode
    {
        public FormNode Previous
        {
            get => _previous;
            set {
                _previous = value;
                value._next = this;
            }
        }
        private FormNode _previous;
        public FormNode Next
        {
            get => _next;
            set {
                _next = value;
                value._previous = this;
            }
        }
        private FormNode _next;

        public Vector2 StartPoint => Previous.EndPoint;
        public Vector2 EndPoint { get; set; }
        public abstract int DetailLevel { get; }

        public abstract Vector2 SamplePoint(float t);
    }
}
