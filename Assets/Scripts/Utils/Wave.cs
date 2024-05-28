using UnityEngine;

namespace Utils
{
    public class Wave
    {
        public float Origin { get; }
        public float Spread { get; }
        public float Frequency { get; }
        public float Lifespan { get; }
        public bool Idle { get; private set; }

        private float _timer;
        private float _progress;
        private float _width;
        private float _height;

        public Wave(float origin, float spread, float frequency, float lifespan)
        {
            Origin = origin;
            Spread = spread;
            Frequency = frequency;
            Lifespan = lifespan;
        }

        public float SamplePoint(float point)
        {
            var localPoint = Mathf.Abs(point - Origin);
            return localPoint < _width
                ? -Mathf.Cos(localPoint * Frequency - _progress * Spread) * (1f - localPoint / _width) * _height * 1.5f
                : 0f;
        }

        public void Update(float deltaTime)
        {
            _timer = Mathf.Min(_timer + deltaTime, Lifespan);
            _progress = _timer / Lifespan;
            _width = Mathf.PI / (Frequency * 2f) + _progress * Spread / Frequency;
            _height = Mathf.Min(_timer * 10f, Mathf.Pow(1f - _progress, 3f));
            Idle = Mathf.Approximately(_timer, Lifespan);
        }
    }
}
