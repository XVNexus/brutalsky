using UnityEngine;

namespace Utils
{
    public class WaveEffect
    {
        public float origin { get; private set; }
        public float spread { get; private set; }
        public float frequency { get; private set; }
        public float lifespan { get; private set; }
        public float scale { get; private set; }
        public bool active => timer < lifespan;

        private float timer;
        private float progress;
        private float width;
        private float height;

        public WaveEffect(float origin, float spread, float frequency, float lifespan, float scale = 1f)
        {
            this.origin = origin;
            this.spread = spread;
            this.frequency = frequency;
            this.lifespan = lifespan;
            this.scale = scale;
        }

        public float SamplePoint(float point)
        {
            var localPoint = Mathf.Abs(point - origin);
            return localPoint < width
                ? Mathf.Cos(localPoint * frequency - progress * spread) * height * scale
                : 0f;
        }

        public void Update(float deltaTime)
        {
            timer = Mathf.Min(timer + deltaTime, lifespan);
            progress = timer / lifespan;
            width = Mathf.PI / (frequency * 2f) + progress * spread / frequency;
            height = Mathf.Min(timer * 10f, Mathf.Pow(1f - progress, 3f));
        }
    }
}
